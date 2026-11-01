using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class PagoExitoso : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UltimaTransaccion"] != null)
                {
                    lblNumeroTransaccion.Text = Session["UltimaTransaccion"].ToString();
                }
            }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Catalogo.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnGarage_Click(object sender, EventArgs e)
        {
            // Redirige a MiGaraje.aspx para que el cliente vea el vehículo recién comprado
            Response.Redirect("MiGaraje.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
