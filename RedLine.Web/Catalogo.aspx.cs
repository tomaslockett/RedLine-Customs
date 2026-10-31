using Redline.Be;
using RedLine.Bll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class _Default : BasePage
    {
        private readonly BLL_Auto _bllAuto = new BLL_Auto();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LlenarFiltros();
                CargarCatalogo();
            }
        }
        private void LlenarFiltros()
        {
            List<AutoBase> lista = _bllAuto.MostrarAutosBase();
            var a = (from s in lista select s.Marca).ToList();
            a.Add("--");
            ddlFiltroMarca.DataSource = a;
            ddlFiltroMarca.DataBind();
            ddlFiltroMarca.Text = "--";
            List<string> preciosFiltro = new List<string> { "--", "0-100.000", "100.000-200.000", "200.000-300.000",
            "300.000-400.000", "400.000-500.000", "500.000+"};
            ddlFiltroPrecio.DataSource = preciosFiltro;
            ddlFiltroPrecio.DataBind();
        }
        private void CargarCatalogo()
        {
            List<AutoBase> lista = _bllAuto.MostrarAutosBase();

            if(ddlFiltroMarca.Text != "--")
            {
                lista = (from auto in lista where auto.Marca == ddlFiltroMarca.Text select auto).ToList();
            }
            if(ddlFiltroPrecio.Text != "--")
            {
                if(ddlFiltroPrecio.Text == "500.000+")
                {
                    lista = (from auto in lista where auto.PrecioBase >= 500000 select auto).ToList();
                }
                else
                {
                    string[] numerosSeparadosFiltro = ddlFiltroPrecio.Text.Split('-');
                    //limpiamos el punto de el medio de los numeros para q no se piensen q es un numero con ,

                    int numeroMenor = int.Parse(numerosSeparadosFiltro[0].Replace(".", ""));
                    int numeroMayor = int.Parse(numerosSeparadosFiltro[1].Replace(".", ""));

                    //ahora si aplicamos el filtro

                    lista = (from auto in lista where auto.PrecioBase >= numeroMenor && auto.PrecioBase <= numeroMayor select auto).ToList();
                }
            }

            repAutos.DataSource = lista;
            repAutos.DataBind();

           

        }

        protected void btnAplicarFiltros_Click(object sender, EventArgs e)
        {
            CargarCatalogo();
        }
    }
}