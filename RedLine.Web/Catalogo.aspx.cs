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

            // Marcas únicas; el índice 0 conserva ListItem("Marca", Value="")
            // para que BasePage.TraducirItemsDropDownList pueda traducirlo.
            List<string> marcas = (from s in lista select s.Marca).Distinct().ToList();
            ddlFiltroMarca.DataSource = marcas;
            ddlFiltroMarca.DataBind();
            ddlFiltroMarca.Items.Insert(0, new ListItem("Marca", ""));
            ddlFiltroMarca.SelectedIndex = 0;

            List<string> preciosFiltro = new List<string> { "0-100.000", "100.000-200.000", "200.000-300.000",
            "300.000-400.000", "400.000-500.000", "500.000+"};
            ddlFiltroPrecio.DataSource = preciosFiltro;
            ddlFiltroPrecio.DataBind();
            ddlFiltroPrecio.Items.Insert(0, new ListItem("Precio", ""));
            ddlFiltroPrecio.SelectedIndex = 0;
        }
        private void CargarCatalogo()
        {
            List<AutoBase> lista = _bllAuto.MostrarAutosBase();

            // Filtro por marca: Value vacío ("Marca") significa "todas"
            string marca = ddlFiltroMarca.SelectedValue;
            if (!string.IsNullOrEmpty(marca))
            {
                lista = (from auto in lista where auto.Marca == marca select auto).ToList();
            }

            // Filtro por rango de precios: Value vacío ("Precio") significa "todos"
            string rango = ddlFiltroPrecio.SelectedValue;
            if (!string.IsNullOrEmpty(rango))
            {
                if(rango == "500.000+")
                {
                    lista = (from auto in lista where auto.PrecioBase >= 500000 select auto).ToList();
                }
                else
                {
                    string[] numerosSeparadosFiltro = rango.Split('-');
                    //limpiamos el punto de el medio de los numeros para q no se piensen q es un numero con ,

                    int numeroMenor = int.Parse(numerosSeparadosFiltro[0].Replace(".", ""));
                    int numeroMayor = int.Parse(numerosSeparadosFiltro[1].Replace(".", ""));

                    //ahora si aplicamos el filtro

                    lista = (from auto in lista where auto.PrecioBase >= numeroMenor && auto.PrecioBase <= numeroMayor select auto).ToList();
                }
            }

            // Búsqueda por marca o modelo (barra superior / botón Aplicar)
            string textoBuscado = txtBuscarCatalogo != null && txtBuscarCatalogo.Text != null
                ? txtBuscarCatalogo.Text.Trim()
                : string.Empty;
            if (!string.IsNullOrEmpty(textoBuscado))
            {
                string objetivo = textoBuscado.ToLower();
                lista = (from auto in lista
                         where (auto.Marca != null && auto.Marca.ToLower().Contains(objetivo))
                            || (auto.Modelo != null && auto.Modelo.ToLower().Contains(objetivo))
                         select auto).ToList();
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