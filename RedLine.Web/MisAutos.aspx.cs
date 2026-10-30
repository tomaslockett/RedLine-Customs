using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.BLL;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class MisAutos : BasePage
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

            // 1. Obtener desde el garage del cliente en sesión
            if (cliente.Garage != null && cliente.Garage.Count > 0)
            {
                foreach (var a in cliente.Garage)
                {
                    AsegurarAutoValido(a);
                    if (!autosComprados.Any(item => item.ID == a.ID || (!string.IsNullOrEmpty(a.NumeroFactura) && item.NumeroFactura == a.NumeroFactura)))
                    {
                        autosComprados.Add(a);
                    }
                }
            }

            // 2. Obtener desde las ventas registradas para este cliente
            string identificador = !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : cliente.Email;
            var ventasCliente = _bllVenta.ListarPorCliente(identificador);
            foreach (var v in ventasCliente)
            {
                if (v.AutoPersonalizado != null)
                {
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

                    if (!autosComprados.Any(a => a.ID == v.AutoPersonalizado.ID || (!string.IsNullOrEmpty(a.NumeroFactura) && a.NumeroFactura == v.AutoPersonalizado.NumeroFactura)))
                    {
                        autosComprados.Add(v.AutoPersonalizado);
                    }
                }
            }

            // 3. Si acaba de comprar en esta sesión
            if (Session["UltimaVenta"] is Venta ultimaVenta && ultimaVenta.AutoPersonalizado != null)
            {
                AsegurarAutoValido(ultimaVenta.AutoPersonalizado);
                if (!autosComprados.Any(a => a.NumeroVenta == ultimaVenta.NumeroVenta))
                {
                    autosComprados.Insert(0, ultimaVenta.AutoPersonalizado);
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

                if (factura == null)
                {
                    var venta = _bllVenta.ObtenerPorNumeroFactura(numeroFactura);
                    factura = venta?.Factura;
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
                html += $"<span class='tag-mejora'><i class='fa-solid fa-wrench'></i> {item.Trim()}</span> ";
            }
            return html;
        }

        private Cliente ObtenerClienteActual()
        {
            if (Session["ClienteSession"] != null)
            {
                return (Cliente)Session["ClienteSession"];
            }

            if (SessionManager.Instancia.IsLogged())
            {
                var usuario = SessionManager.Instancia.Usuario;
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

            // Cliente predeterminado si es visitante o explorador
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
