using Redline.Be;
using RedLine.Bll;
using System;
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
                CargarCatalogo();
            }
        }

        private void CargarCatalogo()
        {
            List<AutoBase> lista = _bllAuto.MostrarAutosBase();
            repAutos.DataSource = lista;
            repAutos.DataBind();
        }

        protected void btnAplicarFiltros_Click(object sender, EventArgs e)
        {
            CargarCatalogo();
        }
    }
}