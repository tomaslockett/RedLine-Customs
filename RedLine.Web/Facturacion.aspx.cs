using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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
                phFacturaVacia.Visible = true;
                phFacturaDetalle.Visible = false;
                return;
            }

            phFacturaVacia.Visible = false;
            phFacturaDetalle.Visible = true;

            // Solo poblar el DropDownList en la carga inicial o si estaba vacío
            if (!IsPostBack || ddlFacturas.Items.Count == 0)
            {
                if (todasLasFacturas.Count > 1)
                {
                    ddlFacturas.Visible = true;
                    ddlFacturas.Items.Clear();
                    foreach (var f in todasLasFacturas)
                    {
                        string numFac = !string.IsNullOrEmpty(f.NumeroFactura) ? f.NumeroFactura : $"FC-B-0001-{f.ID:D8}";
                        DateTime fecha = f.FechaEmision != DateTime.MinValue ? f.FechaEmision : DateTime.Now;
                        string textoItem = $"{numFac} - ${f.Total:N0} ({fecha:dd/MM/yyyy})";
                        ddlFacturas.Items.Add(new ListItem(textoItem, numFac));
                    }
                }
                else
                {
                    ddlFacturas.Visible = false;
                }
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

            if (ddlFacturas.Visible && !string.IsNullOrEmpty(factura.NumeroFactura))
            {
                ListItem itemEncontrado = ddlFacturas.Items.FindByValue(factura.NumeroFactura);
                if (itemEncontrado != null)
                {
                    ddlFacturas.ClearSelection();
                    itemEncontrado.Selected = true;
                }
            }

            Session["FacturaEnPantalla"] = factura;
            RenderizarFactura(factura);
        }

        private void RenderizarFactura(Factura factura)
        {
            if (factura == null) return;

            // Datos del Comprobante
            string numFactura = !string.IsNullOrEmpty(factura.NumeroFactura) ? factura.NumeroFactura : ("FC-B-0001-" + factura.ID.ToString("D8"));
            DateTime fechaEmision = factura.FechaEmision != DateTime.MinValue ? factura.FechaEmision : DateTime.Now;

            lblNumeroFactura.Text = numFactura;
            lblFechaEmision.Text = fechaEmision.ToString("dd/MM/yyyy HH:mm");
            lblNumeroVenta.Text = !string.IsNullOrEmpty(factura.Venta?.NumeroVenta) ? factura.Venta.NumeroVenta : ("VTA-" + factura.ID.ToString("D6"));
            lblMetodoPago.Text = !string.IsNullOrEmpty(factura.MetodoPago) ? factura.MetodoPago : (factura.Venta?.MetodoPago ?? "Tarjeta de Crédito");
            lblVencimientoCae.Text = fechaEmision.AddDays(10).ToString("dd/MM/yyyy");

            // Datos del Cliente
            Cliente cliente = factura.Venta?.Cliente ?? Session["ClienteSession"] as Cliente;
            if (cliente == null && SessionManager.Instancia.IsLogged())
            {
                var u = SessionManager.Instancia.Usuario;
                cliente = new Cliente
                {
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    DNI = u.DNI,
                    Email = u.Email
                };
            }

            if (cliente != null)
            {
                string nombreCompleto = $"{cliente.Nombre} {cliente.Apellido}".Trim();
                lblNombreCliente.Text = !string.IsNullOrEmpty(nombreCompleto) ? nombreCompleto : "Consumidor Final";
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
                decimal precioSinIva = factura.Total > 0 ? Math.Round(factura.Total / 1.21m, 2) : 50000m;
                items.Add(new ItemFacturaViewModel
                {
                    Concepto = "Vehículo Deportivo Custom Edition",
                    Cantidad = 1,
                    PrecioUnitario = precioSinIva,
                    Subtotal = precioSinIva
                });
            }

            var autoPers = factura.Venta?.AutoPersonalizado;
            if (autoPers != null && autoPers.Mejoras != null && autoPers.Mejoras.Count > 0)
            {
                foreach (var extra in autoPers.Mejoras)
                {
                    items.Add(new ItemFacturaViewModel
                    {
                        Concepto = $"Personalización: {extra.Nombre}" + (!string.IsNullOrEmpty(extra.Categoria) ? $" ({extra.Categoria})" : ""),
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
            decimal totalFactura = factura.Total > 0 ? factura.Total : items.Sum(i => i.Subtotal);
            decimal totalNeto = factura.Subtotal > 0 ? factura.Subtotal : Math.Round(totalFactura / 1.21m, 2);
            decimal iva = factura.MontoIVA > 0 ? factura.MontoIVA : (totalFactura - totalNeto);

            lblSubtotalNeto.Text = "$" + totalNeto.ToString("N0");
            lblIvaNeto.Text = "$" + iva.ToString("N0");
            lblTotalFacturado.Text = "$" + totalFactura.ToString("N0");
        }

        private List<Factura> ObtenerTodasLasFacturasDisponibles()
        {
            var resultado = new List<Factura>();

            // 1. De sesión (tienen la referencia completa a Venta y AutoPersonalizado)
            if (Session["UltimaVenta"] is Venta sesVta && sesVta.Factura != null)
            {
                if (sesVta.Factura.Venta == null) sesVta.Factura.Venta = sesVta;
                AgregarFacturaSiNoExiste(resultado, sesVta.Factura);
            }

            if (Session["UltimaFactura"] is Factura sesFac)
            {
                if (sesFac.Venta == null && Session["UltimaVenta"] is Venta vtaAsociada)
                {
                    sesFac.Venta = vtaAsociada;
                }
                AgregarFacturaSiNoExiste(resultado, sesFac);
            }

            // 2. Del cliente en sesión
            if (Session["ClienteSession"] is Cliente cliente)
            {
                string idCliente = !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : cliente.Email;
                var ventasCli = _bllVenta.ListarPorCliente(idCliente);
                if (ventasCli != null)
                {
                    foreach (var v in ventasCli)
                    {
                        if (v?.Factura != null)
                        {
                            if (v.Factura.Venta == null) v.Factura.Venta = v;
                            AgregarFacturaSiNoExiste(resultado, v.Factura);
                        }
                    }
                }
            }

            // 3. De todas las ventas registradas (priorizamos Ventas porque traen el AutoPersonalizado vinculado)
            try
            {
                var vtas = _bllVenta.Listar();
                if (vtas != null)
                {
                    foreach (var v in vtas)
                    {
                        if (v?.Factura != null)
                        {
                            if (v.Factura.Venta == null) v.Factura.Venta = v;
                            AgregarFacturaSiNoExiste(resultado, v.Factura);
                        }
                    }
                }
            }
            catch { }

            // 4. De la tabla/servicio de Facturas
            try
            {
                var facs = _bllFactura.Listar();
                if (facs != null)
                {
                    foreach (var f in facs)
                    {
                        AgregarFacturaSiNoExiste(resultado, f);
                    }
                }
            }
            catch { }

            return resultado.OrderByDescending(f => f.FechaEmision).ToList();
        }

        private void AgregarFacturaSiNoExiste(List<Factura> lista, Factura factura)
        {
            if (factura == null) return;

            var existente = lista.FirstOrDefault(r =>
                (factura.ID > 0 && r.ID == factura.ID) ||
                (!string.IsNullOrEmpty(factura.NumeroFactura) && string.Equals(r.NumeroFactura, factura.NumeroFactura, StringComparison.OrdinalIgnoreCase)));

            if (existente == null)
            {
                lista.Add(factura);
            }
            else if (existente.Venta == null && factura.Venta != null)
            {
                // Si la que ya estaba no tenía los datos de la Venta y la nueva sí, la enriquecemos
                existente.Venta = factura.Venta;
            }
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
            string num = factura?.NumeroFactura ?? ddlFacturas.SelectedValue;
            string vta = factura?.Venta?.NumeroVenta;

            Response.Redirect($"~/DescargarFactura.ashx?num={Server.UrlEncode(num)}&vta={Server.UrlEncode(vta)}", false);
            Context.ApplicationInstance.CompleteRequest();
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
