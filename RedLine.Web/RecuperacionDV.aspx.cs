using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class RecuperacionDV : BasePage
    {
        private readonly BLL_DigitoVerificador blldv = new BLL_DigitoVerificador();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string errores = blldv.VerificarTodaLaBaseDeDatos();

                if (!string.IsNullOrWhiteSpace(errores))
                {
                    log.InnerHtml = errores;
                }
                else
                {
                    lblLogVacio.Text = Traducir("lblLogVacio");
                }
            }
        }

        public void SalirDV(object sender, EventArgs e)
        {
            SessionManager.Instancia.Logout();
            Response.Redirect("LogIn.aspx");
        }

        public void RecalcularDV(object sender, EventArgs e)
        {
            string error = blldv.VerificarTodaLaBaseDeDatos();

            blldv.RecalcularTodaLaBaseDeDatos();
            blldv.RegistrarEventoIntegridadComprometida(error);

            SessionManager.Instancia.Logout();
            Session["Inconsistencia"] = false;
            Response.Redirect("LogIn.aspx");
        }

        public void RestoreDV(object sender, EventArgs e)
        {
            Response.Redirect("BackupRestore.aspx");
        }
    }
}