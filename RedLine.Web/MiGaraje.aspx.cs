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
    public partial class MiGaraje : BasePage
    {
        private readonly BLL_Venta _bllVenta = new BLL_Venta();
        private readonly BLL_Factura _bllFactura = new BLL_Factura();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarGarageCliente();
            }
        }

        private void CargarGarageCliente()
        {
            Cliente cliente = ObtenerClienteActual();

            lblNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellido}".Trim();
            lblDniCliente.Text = !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : "Consumidor Final";

            List<AutoPersonalizado> autosComprados = new List<AutoPersonalizado>();

            // 1. Si acaba de comprar en esta sesión (lo mostramos primero en la lista)
            if (Session["UltimaVenta"] is Venta ultimaVenta && ultimaVenta.AutoPersonalizado != null)
            {
                MapearDatosVentaEnAuto(ultimaVenta);
                AgregarSiNoExiste(autosComprados, ultimaVenta.AutoPersonalizado);
            }

            // 2. Obtener desde las ventas registradas para este cliente
            string identificador = !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : cliente.Email;
            var ventasCliente = _bllVenta.ListarPorCliente(identificador);
            if (ventasCliente != null)
            {
                foreach (var v in ventasCliente)
                {
                    if (v?.AutoPersonalizado != null)
                    {
                        MapearDatosVentaEnAuto(v);
                        AgregarSiNoExiste(autosComprados, v.AutoPersonalizado);
                    }
                }
            }

            // 3. Obtener desde el garage del cliente en sesión
            if (cliente.Garage != null && cliente.Garage.Count > 0)
            {
                foreach (var a in cliente.Garage)
                {
                    AsegurarAutoValido(a);
                    AgregarSiNoExiste(autosComprados, a);
                }
            }

            lblCantidadAutos.Text = autosComprados.Count.ToString();

            if (autosComprados.Count == 0)
            {
                phGarageVacio.Visible = true;
                phGarageLista.Visible = false;
            }
            else
            {
                phGarageVacio.Visible = false;
                phGarageLista.Visible = true;

                repAutosComprados.DataSource = autosComprados;
                repAutosComprados.DataBind();
            }
        }

        private void MapearDatosVentaEnAuto(Venta v)
        {
            if (v?.AutoPersonalizado == null) return;

            if (v.AutoPersonalizado.AuBase == null && v.AutoBase != null)
            {
                v.AutoPersonalizado.AuBase = v.AutoBase;
            }

            if (string.IsNullOrEmpty(v.AutoPersonalizado.NumeroFactura) && v.Factura != null)
            {
                v.AutoPersonalizado.NumeroFactura = v.Factura.NumeroFactura;
            }

            if (string.IsNullOrEmpty(v.AutoPersonalizado.NumeroVenta))
            {
                v.AutoPersonalizado.NumeroVenta = v.NumeroVenta;
            }

            if (v.AutoPersonalizado.TotalPagado <= 0)
            {
                v.AutoPersonalizado.TotalPagado = v.Total;
            }

            if (string.IsNullOrEmpty(v.AutoPersonalizado.MetodoPago))
            {
                v.AutoPersonalizado.MetodoPago = v.MetodoPago;
            }

            if (!v.AutoPersonalizado.FechaCompra.HasValue)
            {
                v.AutoPersonalizado.FechaCompra = v.Fecha;
            }

            AsegurarAutoValido(v.AutoPersonalizado);
        }

        private void AgregarSiNoExiste(List<AutoPersonalizado> lista, AutoPersonalizado auto)
        {
            if (auto == null) return;

            bool existe = lista.Any(item =>
                (auto.ID > 0 && item.ID == auto.ID) ||
                (!string.IsNullOrEmpty(auto.NumeroFactura) && item.NumeroFactura == auto.NumeroFactura) ||
                (!string.IsNullOrEmpty(auto.NumeroVenta) && item.NumeroVenta == auto.NumeroVenta));

            if (!existe)
            {
                lista.Add(auto);
            }
        }

        private void AsegurarAutoValido(AutoPersonalizado auto)
        {
            if (auto == null) return;

            if (auto.AuBase == null)
            {
                auto.AuBase = new AutoBase
                {
                    Marca = "RedLine",
                    Modelo = "Custom Edition",
                    Anio = DateTime.Now.Year,
                    VelocidadMaxima = 260,
                    Potencia = 320,
                    Aceleracion0a100 = 3.8m,
                    PrecioBase = 50000
                };
            }

            if (!auto.FechaCompra.HasValue)
            {
                auto.FechaCompra = DateTime.Now;
            }

            if (string.IsNullOrEmpty(auto.MetodoPago))
            {
                auto.MetodoPago = "Transferencia / Tarjeta";
            }

            if (string.IsNullOrEmpty(auto.NumeroVenta))
            {
                auto.NumeroVenta = auto.ID > 0 ? $"V-{auto.ID:D5}" : "V-00001";
            }

            if (string.IsNullOrEmpty(auto.NumeroFactura))
            {
                auto.NumeroFactura = auto.ID > 0 ? $"FAC-{auto.ID:D5}" : "FAC-00001";
            }

            if (auto.TotalPagado <= 0)
            {
                auto.TotalPagado = auto.PrecioFinal > 0 ? auto.PrecioFinal : 50000m;
            }
        }

        protected void repAutosComprados_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DescargarPdf")
            {
                string numeroFactura = e.CommandArgument?.ToString();
                Factura factura = null;

                if (!string.IsNullOrEmpty(numeroFactura))
                {
                    factura = _bllFactura.ObtenerPorNumero(numeroFactura);
                }

                if (factura == null && Session["UltimaFactura"] is Factura sesFactura)
                {
                    if (sesFactura.NumeroFactura == numeroFactura || string.IsNullOrEmpty(numeroFactura))
                    {
                        factura = sesFactura;
                    }
                }

                if (factura == null && !string.IsNullOrEmpty(numeroFactura))
                {
                    var venta = _bllVenta.ObtenerPorNumeroFactura(numeroFactura);
                    factura = venta?.Factura;
                }

                if (factura != null)
                {
                    byte[] pdfBytes = PdfFacturaServicio.GenerarPdfFactura(factura);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", $"attachment; filename=Factura_{factura.NumeroFactura}.pdf");
                    Response.BinaryWrite(pdfBytes);
                    Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    MostrarAlertaTraducida("msg_factura_no_encontrada");
                }
            }
        }

        public string ObtenerImagenAuto(object autoObj)
        {
            if (autoObj is AutoBase auto && auto.ImagenBinaria != null && auto.ImagenBinaria.Length > 0)
            {
                return "data:image/jpeg;base64," + Convert.ToBase64String(auto.ImagenBinaria);
            }
            return "Content/img/porsche.jfif";
        }

        public string FormatearTagsMejoras(object nombreExtraObj)
        {
            string extras = nombreExtraObj?.ToString();
            if (string.IsNullOrEmpty(extras) || extras == "Configuración Estándar")
            {
                return "<span class='tag-mejora' style='background:#1e293b; color:#94a3b8; border-color:#334155;'><i class='fa-solid fa-car'></i> " + Traducir("texto_configuracion_fabrica") + "</span>";
            }

            string[] items = extras.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string html = string.Empty;
            foreach (var item in items)
            {
                html += $"<span class='tag-mejora'><i class='fa-solid fa-wrench'></i> {HttpUtility.HtmlEncode(item.Trim())}</span> ";
            }
            return html;
        }

        private Cliente ObtenerClienteActual()
        {
            if (SessionManager.Instancia.IsLogged())
            {
                var usuario = SessionManager.Instancia.Usuario;

                if (Session["ClienteSession"] is Cliente clienteEnSesion && clienteEnSesion.ID == usuario.ID)
                {
                    return clienteEnSesion;
                }

                var cliente = new Cliente
                {
                    ID = usuario.ID,
                    DNI = usuario.DNI,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    Telefono = "1140003000",
                    Direccion = "Buenos Aires, Argentina"
                };
                Session["ClienteSession"] = cliente;
                return cliente;
            }

            if (Session["ClienteSession"] is Cliente sesionExistente)
            {
                return sesionExistente;
            }

            var clienteVisitante = new Cliente
            {
                ID = 999,
                DNI = "35444888",
                Nombre = "Cliente",
                Apellido = "RedLine",
                Email = "cliente@redlinecustoms.com",
                Telefono = "1155554444",
                Direccion = "Av. del Libertador 4500, CABA"
            };
            Session["ClienteSession"] = clienteVisitante;
            return clienteVisitante;
        }
    }
}