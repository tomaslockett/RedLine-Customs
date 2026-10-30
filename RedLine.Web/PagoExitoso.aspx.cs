using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Web;
using System.Web.UI;

namespace RedLine.Web
{
    public partial class PagoExitoso : BasePage
    {
        private readonly BLL_Factura _bllFactura = new BLL_Factura();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UltimaTransaccion"] != null)
                {
                    lblNumeroTransaccion.Text = Session["UltimaTransaccion"].ToString();
                }
            }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Catalogo.aspx");
        }

        protected void btnGarage_Click(object sender, EventArgs e)
        {
            Response.Redirect("MisAutos.aspx");
        }

        protected void btnVerFactura_Click(object sender, EventArgs e)
        {
            var factura = Session["UltimaFactura"] as Factura;
            if (factura != null)
            {
                Response.Redirect($"Facturacion.aspx?num={factura.NumeroFactura}");
            }
            else
            {
                Response.Redirect("Facturacion.aspx");
            }
        }

        protected void btnDescargarPdf_Click(object sender, EventArgs e)
        {
            var factura = Session["UltimaFactura"] as Factura;
            if (factura == null && Session["UltimaTransaccion"] != null)
            {
                var venta = Session["UltimaVenta"] as Venta;
                factura = venta?.Factura;
            }

            if (factura != null)
            {
                byte[] pdf = PdfFacturaServicio.GenerarPdfFactura(factura);

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=Factura_{factura.NumeroFactura}.pdf");
                Response.BinaryWrite(pdf);
                Response.Flush();
                Response.End();
            }
            else
            {
                MostrarAlertaTraducida("msg_factura_no_encontrada");
            }
        }
    }
}
