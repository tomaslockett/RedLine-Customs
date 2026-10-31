using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class HistorialVentas : BasePage
    {
        private readonly BLL_Venta _bllVenta = new BLL_Venta();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarGrillaEIndicadores();
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarGrillaEIndicadores();
        }

        /// <summary>
        /// Obtiene el listado de ventas desde BLL_Venta, aplica los filtros de búsqueda y fechas
        /// de forma simple, calcula los indicadores (KPIs) y enlaza la grilla dgvVentas.
        /// </summary>
        private void CargarGrillaEIndicadores()
        {
            try
            {
                List<Venta> todasLasVentas = _bllVenta.Listar();
                if (todasLasVentas == null)
                {
                    todasLasVentas = new List<Venta>();
                }

                string busqueda = txtBuscar.Text.Trim().ToLower();
                bool tieneFechaDesde = DateTime.TryParse(txtFechaDesde.Text, out DateTime fechaDesde);
                bool tieneFechaHasta = DateTime.TryParse(txtFechaHasta.Text, out DateTime fechaHasta);

                List<Venta> listaFiltrada = new List<Venta>();

                foreach (Venta v in todasLasVentas)
                {
                    if (v == null)
                    {
                        continue;
                    }

                    // Aseguramos objetos anidados no nulos para que Eval("Cliente.Nombre"), Eval("AutoBase.Marca")
                    // y Eval("AutoPersonalizado.NombreExtra") en el GridView no lancen excepción.
                    AsegurarDatosParaGrilla(v);

                    // 1. Filtro por texto (N° de venta, nombre/apellido del cliente o marca/modelo del vehículo)
                    if (!string.IsNullOrEmpty(busqueda))
                    {
                        string numVenta = (v.NumeroVenta ?? string.Empty).ToLower();
                        string nombreCliente = ((v.Cliente.Nombre ?? string.Empty) + " " + (v.Cliente.Apellido ?? string.Empty)).ToLower();
                        string dniCliente = (v.Cliente.DNI ?? string.Empty).ToLower();
                        string vehiculo = ((v.AutoBase.Marca ?? string.Empty) + " " + (v.AutoBase.Modelo ?? string.Empty)).ToLower();

                        bool coincideTexto = numVenta.Contains(busqueda) ||
                                             nombreCliente.Contains(busqueda) ||
                                             dniCliente.Contains(busqueda) ||
                                             vehiculo.Contains(busqueda);

                        if (!coincideTexto)
                        {
                            continue;
                        }
                    }

                    // 2. Filtro por Fecha Desde
                    if (tieneFechaDesde && v.Fecha.Date < fechaDesde.Date)
                    {
                        continue;
                    }

                    // 3. Filtro por Fecha Hasta
                    if (tieneFechaHasta && v.Fecha.Date > fechaHasta.Date)
                    {
                        continue;
                    }

                    listaFiltrada.Add(v);
                }

                // Cálculo de KPIs con bucle simple
                if (listaFiltrada.Count > 0)
                {
                    decimal ingresosTotales = 0m;
                    int ventasMesActual = 0;
                    int mesActual = DateTime.Now.Month;
                    int anioActual = DateTime.Now.Year;

                    foreach (Venta v in listaFiltrada)
                    {
                        ingresosTotales += v.Total;

                        if (v.Fecha.Month == mesActual && v.Fecha.Year == anioActual)
                        {
                            ventasMesActual++;
                        }
                    }

                    decimal promedio = ingresosTotales / listaFiltrada.Count;

                    lblVentasTotales.Text = listaFiltrada.Count.ToString();
                    lblIngresosTotales.Text = $"US$ {ingresosTotales:N2}";
                    lblTicketPromedio.Text = $"US$ {promedio:N2}";
                    lblVentasEsteMes.Text = ventasMesActual.ToString();
                }
                else
                {
                    lblVentasTotales.Text = "0";
                    lblIngresosTotales.Text = "US$ 0,00";
                    lblTicketPromedio.Text = "US$ 0,00";
                    lblVentasEsteMes.Text = "0";
                }

                dgvVentas.DataSource = listaFiltrada;
                dgvVentas.DataBind();
            }
            catch (Exception)
            {
                MostrarAlertaTraducida("msg_error_cargar_ventas");
            }
        }

        /// <summary>
        /// Garantiza que una Venta tenga instancias válidas de Cliente, AutoBase y AutoPersonalizado
        /// antes de enlazarla al GridView.
        /// </summary>
        private void AsegurarDatosParaGrilla(Venta v)
        {
            if (v.Cliente == null)
            {
                v.Cliente = new Cliente
                {
                    Nombre = "Consumidor",
                    Apellido = "Final",
                    DNI = "-"
                };
            }

            if (v.AutoBase == null)
            {
                if (v.AutoPersonalizado != null && v.AutoPersonalizado.AuBase != null)
                {
                    v.AutoBase = v.AutoPersonalizado.AuBase;
                }
                else
                {
                    v.AutoBase = new AutoBase
                    {
                        Marca = "RedLine",
                        Modelo = "Custom Edition"
                    };
                }
            }

            if (v.AutoPersonalizado == null)
            {
                v.AutoPersonalizado = new AutoPersonalizado
                {
                    AuBase = v.AutoBase,
                    NombreExtra = Traducir("texto_estandar_base")
                };
            }
            else if (string.IsNullOrEmpty(v.AutoPersonalizado.NombreExtra) || v.AutoPersonalizado.NombreExtra == "Configuración Estándar")
            {
                v.AutoPersonalizado.NombreExtra = Traducir("texto_estandar_base");
            }
        }
    }
}
