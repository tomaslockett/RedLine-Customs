using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace RedLine.Web
{
    /// <summary>
    /// Handler de descarga del Informe Mensual de Ventas en PDF.
    /// Replica el mecanismo de DescargarFactura.ashx: IHttpHandler + IRequiresSessionState
    /// (para respetar la sesión y el idioma activo del usuario), validación de autenticación,
    /// parámetros ?mes=&anio=&modo= (ver = inline, vacío = descarga) y respuesta binaria.
    /// </summary>
    public class DescargarInformeVentas : IHttpHandler, IRequiresSessionState
    {
        private readonly BLL_Venta _bllVenta = new BLL_Venta();

        public void ProcessRequest(HttpContext context)
        {
            // 1. Autenticación (igual criterio que el resto del sistema: SessionManager)
            if (!SessionManager.Instancia.IsLogged() || SessionManager.Instancia.Usuario == null)
            {
                ResponderError(context, 403, "Acceso denegado. Debe iniciar sesion para descargar el informe de ventas.");
                return;
            }

            // 2. Validación de período
            int mes;
            int anio;
            if (!int.TryParse(context.Request.QueryString["mes"], out mes) || mes < 1 || mes > 12 ||
                !int.TryParse(context.Request.QueryString["anio"], out anio) || anio < 1900 || anio > 2100)
            {
                ResponderError(context, 400, "Periodo (mes/anio) invalido para el informe de ventas.");
                return;
            }

            // 3. Cálculo del informe reutilizando BLL_Venta (aún no hay nuevas capas DAL/BLL)
            List<Venta> ventas = _bllVenta.Listar() ?? new List<Venta>();
            InformeVentasServicio.DatosInformeVentas datos = InformeVentasServicio.Calcular(ventas, mes, anio);

            // 4. Estado vacío: no hay ventas en el período -> mensaje traducido
            if (datos == null || datos.Unidades == 0)
            {
                string mensajeVacio = SubjectIdioma.Instancia.Traducir("msg_informe_sin_datos");
                ResponderError(context, 404, string.IsNullOrEmpty(mensajeVacio) || mensajeVacio.StartsWith("[")
                    ? "No hay ventas registradas en el periodo seleccionado."
                    : mensajeVacio);
                return;
            }

            // 5. Generación del PDF y respuesta binaria (adjunta o inline con modo=ver)
            byte[] pdfBytes = InformeVentasServicio.GenerarPdfInformeVentas(datos);
            string disposicion = string.Equals(context.Request.QueryString["modo"], "ver", StringComparison.OrdinalIgnoreCase)
                ? "inline"
                : "attachment";

            string nombreArchivo = "InformeVentas_" + datos.Anio.ToString("D4") + datos.Mes.ToString("D2") + ".pdf";

            context.Response.Clear();
            context.Response.Buffer = true;
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", disposicion + "; filename=" + nombreArchivo);
            context.Response.BinaryWrite(pdfBytes);
            context.Response.Flush();
            context.ApplicationInstance.CompleteRequest();
        }

        private static void ResponderError(HttpContext context, int statusCode, string mensaje)
        {
            context.Response.Clear();
            context.Response.ContentType = "text/plain; charset=utf-8";
            context.Response.StatusCode = statusCode;
            context.Response.Write(mensaje);
            context.Response.Flush();
            context.ApplicationInstance.CompleteRequest();
        }

        public bool IsReusable => false;
    }
}
