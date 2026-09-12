using RedLine.Be.Entidades;
using RedLine.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RedLine.BLL;

//Falta hacer todo lo relacionado con venta!!


namespace RedLine.Web
{
    public partial class HistorialVentas : BasePage
    {

        //private readonly BLL_Venta _bllVenta = new BLL_Venta();

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        CargarGrillaEIndicadores();
        //    }
        //}

        //protected void btnFiltrar_Click(object sender, EventArgs e)
        //{
        //    CargarGrillaEIndicadores();
        //}

        //private void CargarGrillaEIndicadores()
        //{
        //    try
        //    {
        //        List<Venta> listaVentas = _bllVenta.Listar();

        //        string busqueda = txtBuscar.Text.Trim().ToLower();
        //        if (!string.IsNullOrEmpty(busqueda))
        //        {
        //            listaVentas = listaVentas.Where(v =>
        //                (v.NumeroVenta != null && v.NumeroVenta.ToLower().Contains(busqueda)) ||
        //                (v.Cliente != null && (v.Cliente.Nombre.ToLower().Contains(busqueda) || v.Cliente.Apellido.ToLower().Contains(busqueda))) ||
        //                (v.AutoBase != null && (v.AutoBase.Marca.ToLower().Contains(busqueda) || v.AutoBase.Modelo.ToLower().Contains(busqueda)))
        //            ).ToList();
        //        }

        //        if (DateTime.TryParse(txtFechaDesde.Text, out DateTime fechaDesde))
        //        {
        //            listaVentas = listaVentas.Where(v => v.Fecha.Date >= fechaDesde.Date).ToList();
        //        }

        //        if (DateTime.TryParse(txtFechaHasta.Text, out DateTime fechaHasta))
        //        {
        //            listaVentas = listaVentas.Where(v => v.Fecha.Date <= fechaHasta.Date).ToList();
        //        }

        //        // Cálculo de KPIs
        //        if (listaVentas.Count > 0)
        //        {
        //            lblVentasTotales.Text = listaVentas.Count.ToString();

        //            decimal ingresosTotales = listaVentas.Sum(v => v.Total);
        //            lblIngresosTotales.Text = $"US$ {ingresosTotales:N2}";

        //            decimal promedio = listaVentas.Average(v => v.Total);
        //            lblTicketPromedio.Text = $"US$ {promedio:N2}";

        //            int ventasMes = listaVentas.Count(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year);
        //            lblVentasEsteMes.Text = ventasMes.ToString();
        //        }
        //        else
        //        {
        //            lblVentasTotales.Text = "0";
        //            lblIngresosTotales.Text = "US$ 0,00";
        //            lblTicketPromedio.Text = "US$ 0,00";
        //            lblVentasEsteMes.Text = "0";
        //        }

        //        dgvVentas.DataSource = listaVentas;
        //        dgvVentas.DataBind();
        //    }
        //    catch (Exception)
        //    {
        //        MostrarAlertaTraducida("msg_error_cargar_ventas");
        //    }
        //}
    }
}