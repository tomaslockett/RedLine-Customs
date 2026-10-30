using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class Facturacion : BasePage
    {
        private readonly BLL_Factura _bllFactura = new BLL_Factura();
        private readonly BLL_Venta _bllVenta = new BLL_Venta();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosFactura();
            }
        }

        private void CargarDatosFactura(string numeroFacturaSeleccionada = null)
        {
            var todasLasFacturas = ObtenerTodasLasFacturasDisponibles();

            if (todasLasFacturas.Count == 0)
            {
                // No hay facturas emitidas todavía: mostrar estado vacío amigable
                phFacturaVacia.Visible = true;
                phFacturaDetalle.Visible = false;
                return;
            }

            phFacturaVacia.Visible = false;
            phFacturaDetalle.Visible = true;

            // Si hay más de una factura, configurar selector
            if (todasLasFacturas.Count > 1)
            {
                ddlFacturas.Visible = true;
                ddlFacturas.Items.Clear();
                foreach (var f in todasLasFacturas)
                {
                    string textoItem = $"{f.NumeroFactura} - ${f.Total:N0} ({f.FechaEmision:dd/MM/yyyy})";
                    ddlFacturas.Items.Add(new ListItem(textoItem, f.NumeroFactura));
                }
            }
            else
            {
                ddlFacturas.Visible = false;
            }

            Factura factura = null;

            if (!string.IsNullOrEmpty(numeroFacturaSeleccionada))
            {
                factura = todasLasFacturas.FirstOrDefault(f => string.Equals(f.NumeroFactura, numeroFacturaSeleccionada, StringComparison.OrdinalIgnoreCase));
            }

            if (factura == null)
            {
                string numQuery = Request.QueryString["num"];
                if (!string.IsNullOrEmpty(numQuery))
                {
                    factura = todasLasFacturas.FirstOrDefault(f => string.Equals(f.NumeroFactura, numQuery, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (factura == null && Session["UltimaFactura"] is Factura sesFac)
            {
                factura = todasLasFacturas.FirstOrDefault(f => string.Equals(f.NumeroFactura, sesFac.NumeroFactura, StringComparison.OrdinalIgnoreCase)) ?? sesFac;
            }

            if (factura == null)
            {
                factura = todasLasFacturas.First();
            }

            if (ddlFacturas.Visible && ddlFacturas.Items.FindByValue(factura.NumeroFactura) != null)
            {
                ddlFacturas.SelectedValue = factura.NumeroFactura;
            }

            Session["FacturaEnPantalla"] = factura;
            RenderizarFactura(factura);
        }

        private void RenderizarFactura(Factura factura)
        {
            if (factura == null) return;

            // Datos del Comprobante
            lblNumeroFactura.Text = factura.NumeroFactura ?? ("FC-B-0001-" + factura.ID.ToString("D8"));
            lblFechaEmision.Text = factura.FechaEmision != DateTime.MinValue ? factura.FechaEmision.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lblNumeroVenta.Text = factura.Venta?.NumeroVenta ?? ("VTA-" + factura.ID.ToString("D6"));
            lblMetodoPago.Text = factura.MetodoPago ?? factura.Venta?.MetodoPago ?? "Tarjeta de Crédito";
            lblVencimientoCae.Text = DateTime.Now.AddDays(10).ToString("dd/MM/yyyy");

            // Datos del Cliente
            Cliente cliente = factura.Venta?.Cliente ?? Session["ClienteSession"] as Cliente;
            if (cliente != null)
            {
                lblNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellido}".Trim();
                lblDniCliente.Text = !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : "Consumidor Final";
                lblEmailCliente.Text = !string.IsNullOrEmpty(cliente.Email) ? cliente.Email : "-";
            }
            else
            {
                lblNombreCliente.Text = "Consumidor Final";
                lblDniCliente.Text = "-";
                lblEmailCliente.Text = "-";
            }

            // Items Facturados
            var items = new List<ItemFacturaViewModel>();

            var autoBase = factura.Venta?.AutoBase ?? factura.Venta?.AutoPersonalizado?.AuBase;
            if (autoBase != null)
            {
                items.Add(new ItemFacturaViewModel
                {
                    Concepto = $"Vehículo Deportivo: {autoBase.Marca} {autoBase.Modelo} ({autoBase.Anio}) - {autoBase.Potencia} HP",
                    Cantidad = 1,
                    PrecioUnitario = autoBase.PrecioBase,
                    Subtotal = autoBase.PrecioBase
                });
            }
            else
            {
                items.Add(new ItemFacturaViewModel
                {
                    Concepto = "Vehículo Deportivo Custom Edition",
                    Cantidad = 1,
                    PrecioUnitario = Math.Round(factura.Total / 1.21m, 2),
                    Subtotal = Math.Round(factura.Total / 1.21m, 2)
                });
            }

            var autoPers = factura.Venta?.AutoPersonalizado;
            if (autoPers != null && autoPers.Mejoras != null && autoPers.Mejoras.Count > 0)
            {
                foreach (var extra in autoPers.Mejoras)
                {
                    items.Add(new ItemFacturaViewModel
                    {
                        Concepto = $"Personalización: {extra.Nombre} ({extra.Categoria})",
                        Cantidad = 1,
                        PrecioUnitario = extra.Precio,
                        Subtotal = extra.Precio
                    });
                }
            }
            else if (!string.IsNullOrEmpty(autoPers?.NombreExtra) && autoPers.NombreExtra != "Configuración Estándar")
            {
                string[] parts = autoPers.NombreExtra.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    items.Add(new ItemFacturaViewModel
                    {
                        Concepto = "Personalización: " + p.Trim(),
                        Cantidad = 1,
                        PrecioUnitario = 0,
                        Subtotal = 0
                    });
                }
            }

            repItemsFactura.DataSource = items;
            repItemsFactura.DataBind();

            // Totales
            decimal totalNeto = factura.Subtotal > 0 ? factura.Subtotal : Math.Round(factura.Total / 1.21m, 2);
            decimal iva = factura.MontoIVA > 0 ? factura.MontoIVA : (factura.Total - totalNeto);

            lblSubtotalNeto.Text = "$" + totalNeto.ToString("N0");
            lblIvaNeto.Text = "$" + iva.ToString("N0");
            lblTotalFacturado.Text = "$" + factura.Total.ToString("N0");
        }

        private List<Factura> ObtenerTodasLasFacturasDisponibles()
        {
            var resultado = new List<Factura>();

            // 1. De sesión
            if (Session["UltimaFactura"] is Factura sesFac)
            {
                resultado.Add(sesFac);
            }
            if (Session["UltimaVenta"] is Venta sesVta && sesVta.Factura != null)
            {
                if (!resultado.Any(f => f.NumeroFactura == sesVta.Factura.NumeroFactura))
                {
                    resultado.Add(sesVta.Factura);
                }
            }

            // 2. Del cliente en sesión
            if (Session["ClienteSession"] is Cliente cliente)
            {
                string idCliente = !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : cliente.Email;
                var ventasCli = _bllVenta.ListarPorCliente(idCliente);
                foreach (var v in ventasCli)
                {
                    if (v.Factura != null && !resultado.Any(f => f.NumeroFactura == v.Factura.NumeroFactura))
                    {
                        resultado.Add(v.Factura);
                    }
                }
            }

            // 3. De la base de datos y memoria global
            try
            {
                var facs = _bllFactura.Listar();
                if (facs != null)
                {
                    foreach (var f in facs)
                    {
                        if (!resultado.Any(r => r.ID == f.ID || (!string.IsNullOrEmpty(f.NumeroFactura) && r.NumeroFactura == f.NumeroFactura)))
                        {
                            resultado.Add(f);
                        }
                    }
                }
            }
            catch { }

            try
            {
                var vtas = _bllVenta.Listar();
                if (vtas != null)
                {
                    foreach (var v in vtas)
                    {
                        if (v.Factura != null && !resultado.Any(r => r.NumeroFactura == v.Factura.NumeroFactura))
                        {
                            resultado.Add(v.Factura);
                        }
                    }
                }
            }
            catch { }

            return resultado.OrderByDescending(f => f.FechaEmision).ToList();
        }

        protected void ddlFacturas_SelectedIndexChanged(object sender, EventArgs e)
        {
            string num = ddlFacturas.SelectedValue;
            if (!string.IsNullOrEmpty(num))
            {
                CargarDatosFactura(num);
            }
        }

        protected void btnDescargarPdf_Click(object sender, EventArgs e)
        {
            Factura factura = Session["FacturaEnPantalla"] as Factura;

            if (factura == null)
            {
                var disponibles = ObtenerTodasLasFacturasDisponibles();
                if (disponibles.Count > 0) factura = disponibles.First();
            }

            if (factura != null)
            {
                byte[] pdfBytes = PdfFacturaServicio.GenerarPdfFactura(factura);

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=Factura_{factura.NumeroFactura}.pdf");
                Response.BinaryWrite(pdfBytes);
                Response.Flush();
                Response.End();
            }
            else
            {
                MostrarAlertaTraducida("msg_factura_no_encontrada");
            }
        }
    }

    public class ItemFacturaViewModel
    {
        public string Concepto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
