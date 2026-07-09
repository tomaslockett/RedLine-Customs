using Redline.Be;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (SessionManager.Instancia.IsLogged())
                {
                    Usuario userBackend = SessionManager.Instancia.Usuario;

                    litNombreUsuario.Text = $"{userBackend.Nombre} {userBackend.Apellido}";

                    EstablecerMenuLogueado();
                }
                else if (Session["ClienteSession"] != null)
                {
                    var clienteLogueado = (Cliente)Session["ClienteSession"];

                    litNombreUsuario.Text = $"{clienteLogueado.Nombre} {clienteLogueado.Apellido}";

                    EstablecerMenuLogueado();
                }
                else
                {
                    litNombreUsuario.Text = "Mi Cuenta";

                    lnkLogin.Visible = true;
                    lnkRegistrarse.Visible = true;

                    lnkCambioPass.Visible = false;
                    lnkLogout.Visible = false;
                }
            }
        }

        private void EstablecerMenuLogueado()
        {
            lnkLogin.Visible = false;
            lnkRegistrarse.Visible = false;

            lnkCambioPass.Visible = true;
            lnkLogout.Visible = true;
        }
    }
}