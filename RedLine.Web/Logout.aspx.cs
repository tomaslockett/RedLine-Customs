using RedLine.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class Logout : BasePage
    {
        private readonly BLL_Usuario _bllUsuario = new BLL_Usuario();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Cierra la sesión activa (ya sea de un Usuario del sistema o de un Cliente)
        /// y redirige a la pantalla de inicio de sesión.
        /// </summary>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (RedLine.Servicios.SessionManager.Instancia.IsLogged())
                {
                    if (Session["Inconsistencia"] != null && (bool)Session["Inconsistencia"])
                    {
                        _bllUsuario.LogoutInconsistente();
                    }
                    else
                    {
                        _bllUsuario.Logout();
                    }
                }

                // Limpiamos también la sesión de cliente por si ingresó como Cliente
                Session["ClienteSession"] = null;
                Session["UserEmail"] = null;
                Session["Inconsistencia"] = false;

                Response.Redirect("~/LogIn.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                lblError.Text = $"{Traducir("msg_error_logout_prefijo")} {ex.Message}";
                lblError.Visible = true;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Catalogo.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}