using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace RedLine.Web
{
    /// <summary>
    /// Descripción breve de DescargarFactura
    /// </summary>
    public class DescargarFactura : IHttpHandler, IRequiresSessionState
    {

        private readonly BLL_Factura _bllFactura = new BLL_Factura();
        private readonly BLL_Venta _bllVenta = new BLL_Venta();

        public void ProcessRequest(HttpContext context)
        {
            string num = context.Request.QueryString["num"];
            string vta = context.Request.QueryString["vta"];
            string modo = context.Request.QueryString["modo"]; // "ver" abre en el navegador, vacío descarga

            Factura factura = null;

            // 1. Buscar por número de factura
            if (!string.IsNullOrEmpty(num))
            {
                var ventaAsociada = _bllVenta.ObtenerPorNumeroFactura(num);
                factura = _bllFactura.ObtenerPorNumero(num) ?? ventaAsociada?.Factura;

                if (factura != null && factura.Venta == null && ventaAsociada != null)
                {
                    factura.Venta = ventaAsociada;
                }
            }

            // 2. Buscar por número de venta
            if (factura == null && !string.IsNullOrEmpty(vta))
            {
                var venta = _bllVenta.ObtenerPorNumero(vta);
                if (venta?.Factura != null)
                {
                    factura = venta.Factura;
                    if (factura.Venta == null) factura.Venta = venta;
                }
            }

            // 3. Respaldo en Sesión (si es una compra recién hecha en memoria)
            if (context.Session != null)
            {
                if (factura == null && context.Session["FacturaEnPantalla"] is Factura facPantalla)
                {
                    if (string.IsNullOrEmpty(num) || string.Equals(facPantalla.NumeroFactura, num, StringComparison.OrdinalIgnoreCase))
                    {
                        factura = facPantalla;
                    }
                }

                if (factura == null && context.Session["UltimaVenta"] is Venta ventaSesion && ventaSesion.Factura != null)
                {
                    if (string.IsNullOrEmpty(num) || string.Equals(ventaSesion.Factura.NumeroFactura, num, StringComparison.OrdinalIgnoreCase))
                    {
                        factura = ventaSesion.Factura;
                        if (factura.Venta == null) factura.Venta = ventaSesion;
                    }
                }

                if (factura == null && context.Session["UltimaFactura"] is Factura facSesion)
                {
                    if (string.IsNullOrEmpty(num) || string.Equals(facSesion.NumeroFactura, num, StringComparison.OrdinalIgnoreCase))
                    {
                        factura = facSesion;
                    }
                }
            }

            // 4. Generar y devolver el PDF
            if (factura != null)
            {
                byte[] pdfBytes = PdfFacturaServicio.GenerarPdfFactura(factura);
                string numArchivo = !string.IsNullOrEmpty(factura.NumeroFactura) ? factura.NumeroFactura : factura.ID.ToString("D8");

                // Si modo=ver -> "inline" (se abre en pestaña nueva). Si no -> "attachment" (descarga directa)
                string disposicion = string.Equals(modo, "ver", StringComparison.OrdinalIgnoreCase) ? "inline" : "attachment";

                context.Response.Clear();
                context.Response.Buffer = true;
                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("Content-Disposition", $"{disposicion}; filename=Factura_{numArchivo}.pdf");
                context.Response.BinaryWrite(pdfBytes);
                context.Response.Flush();
                context.ApplicationInstance.CompleteRequest();
            }
            else
            {
                context.Response.Clear();
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.StatusCode = 404;
                context.Response.Write("No se encontró el comprobante de factura solicitado.");
                context.ApplicationInstance.CompleteRequest();
            }
        }

        public bool IsReusable => false;
    }
}
