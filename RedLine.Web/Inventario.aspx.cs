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
    public partial class Inventario : System.Web.UI.Page
    {
        BLL_Auto BLL_Auto = new BLL_Auto();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<AutoBase> lista = BLL_Auto.MostrarAutosBase();
                if (lista.Count > 0)
                {
                    phInventarioVacio.Visible = false;
                    phInventarioLista.Visible = true;
                    repInventario.DataSource = lista;
                    repInventario.DataBind();
                }
                else
                {
                    phInventarioVacio.Visible = true;
                    phInventarioLista.Visible = false;
                }
            }
           
        }
        protected void IrAAgregarAuto(object sender, EventArgs e)
        {
            Response.Redirect("CrearAuto.aspx");
        }
    }
}