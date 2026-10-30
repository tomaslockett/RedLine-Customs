using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Web;
using System.Web.SessionState;

namespace RedLine.Web
{
    public class DescargarFactura : IHttpHandler, IRequiresSessionState
    {
        private readonly BLL_Factura _bllFactura = new BLL_Factura();
        private readonly BLL_Venta _bllVenta = new BLL_Venta();

        public void ProcessRequest(HttpContext context)
        {
            string num = context.Request.QueryString["num"];
            string vta = context.Request.QueryString["vta"];

            Factura factura = null;

            if (!string.IsNullOrEmpty(num))
            {
                factura = _bllFactura.ObtenerPorNumero(num);
                if (factura == null)
                {
                    var venta = _bllVenta.ObtenerPorNumeroFactura(num);
                    factura = venta?.Factura;
                }
            }

            if (factura == null && !string.IsNullOrEmpty(vta))
            {
                var venta = _bllVenta.ObtenerPorNumero(vta);
                factura = venta?.Factura;
            }

            if (factura == null && context.Session != null)
            {
                factura = context.Session["UltimaFactura"] as Factura;
                if (factura == null && context.Session["UltimaVenta"] is Venta ventaSesion)
                {
                    factura = ventaSesion.Factura;
                }
            }

            if (factura != null)
            {
                byte[] pdfBytes = PdfFacturaServicio.GenerarPdfFactura(factura);

                context.Response.Clear();
                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("Content-Disposition", $"attachment; filename=Factura_{factura.NumeroFactura}.pdf");
                context.Response.BinaryWrite(pdfBytes);
                context.Response.Flush();
                context.Response.End();
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("No se encontró el comprobante de factura solicitado.");
            }
        }

        public bool IsReusable => false;
    }
}
