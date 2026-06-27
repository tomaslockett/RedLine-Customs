using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class HistorialVentas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //private void CargarGrillaYIndicadores()
        //{
        //    // 1. Vas a buscar a tu base de datos la lista de ventas según los filtros
        //    // (Esto dependerá de cómo se llame tu método en la capa de negocio)

        //    //List<Venta> listaVentas = gestorVentas.ObtenerVentas(txtBuscar.Text, txtFechaDesde.Text, txtFechaHasta.Text);

        //    // 2. 📊 CALCULAR INDICADORES (KPIs) USANDO LINQ
        //    if (listaVentas != null && listaVentas.Count > 0)
        //    {
        //        // Total de ventas (Cantidad de elementos en la lista)
        //        lblVentasTotales.Text = listaVentas.Count.ToString();

        //        // Ingresos Totales (Suma la propiedad 'Total' de todos los autos)
        //        decimal totalIngresos = listaVentas.Sum(v => v.Total);
        //        lblIngresosTotales.Text = "US$ " + totalIngresos.ToString("N2");

        //        // Ticket Promedio (Promedio de la propiedad 'Total')
        //        decimal promedio = listaVentas.Average(v => v.Total);
        //        lblTicketPromedio.Text = "US$ " + promedio.ToString("N2");

        //        // Ventas de este mes (Cuenta cuántas ventas coinciden con el mes y año actual)
        //        int ventasMes = listaVentas.Count(v => v.Fecha.Month == DateTime.Now.Month && v.Fecha.Year == DateTime.Now.Year);
        //        lblVentasEsteMes.Text = ventasMes.ToString();
        //    }
        //    else
        //    {
        //        // Si no hay ventas, reseteamos los contadores a cero
        //        lblVentasTotales.Text = "0";
        //        lblIngresosTotales.Text = "US$ 0,00";
        //        lblTicketPromedio.Text = "US$ 0,00";
        //        lblVentasEsteMes.Text = "0";
        //    }

        //    // 3. 🏁 CARGAR LA GRILLA
        //    dgvVentas.DataSource = listaVentas;
        //    dgvVentas.DataBind();
        //}
    }
}