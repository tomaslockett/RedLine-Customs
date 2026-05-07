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
    public partial class _Default : Page
    {
        BLL_Auto BLL_Auto = new BLL_Auto();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<AutoBase> lista = BLL_Auto.MostrarAutosBase();

                repAutos.DataSource = lista;

                repAutos.DataBind();
            }
        }
        protected void Personalizar(object sender, EventArgs e)
        {

        }
    }
}