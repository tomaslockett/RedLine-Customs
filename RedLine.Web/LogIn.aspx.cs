using Redline.Be;
using RedLine.Bll;
using RedLine.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RedLine.Servicios;
using RedLine.Be.Interfaces;

namespace RedLine.Web
{
    public partial class LogIn : BasePage
    {
        private readonly BLL_Cliente gestorCliente = new BLL_Cliente();
        private readonly BLL_Usuario gestorUsuario = new BLL_Usuario();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MostrarMensaje(Traducir("msgIngresaCredenciales"), true);
                return;
            }

            try
            {
                LoginResult resultado = gestorUsuario.Login(email, password);

                if (resultado == LoginResult.ValidUser)
                {
                    Response.Redirect("Catalogo.aspx");
                    return;
                }
                if (resultado == LoginResult.InconsistencyDVWebMaster)
                {
                    Session.Add("Inconsistencia", true);
                    Response.Redirect("RecuperacionDV.aspx");
                    return;
                }
                if (resultado == LoginResult.InconsistencyDVUserNormal)
                {
                    MostrarMensaje(Traducir("msgSistemaNoFunciona"), true);
                }
            }
            catch (LoginException ex)
            {
                try
                {
                    var clientes = gestorCliente.ObtenerClientes();
                    var clienteLogueado = clientes.Find(c =>
                        c.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                        c.Contraseña.Equals(Hashing.Sha256(password)));

                    if (clienteLogueado != null)
                    {
                        Session["ClienteSession"] = clienteLogueado;
                        Session["UserEmail"] = email;
                        Response.Redirect("Catalogo.aspx");
                    }
                    else
                    {
                        ManejarErrorLogin(ex.Result);
                    }
                }
                catch (Exception)
                {
                    ManejarErrorLogin(ex.Result);
                }
            }
            catch (Exception ex)
            {
                string textoError = Traducir("msgErrorTecnico");
                MostrarMensaje($"{textoError}: {ex.Message}", true);
            }
        }

        private void ManejarErrorLogin(LoginResult resultado)
        {
            string mensajeError;
            switch (resultado)
            {
                case LoginResult.InvalidUsername:
                    mensajeError = Traducir("msgUsuarioNoExiste");
                    break;
                case LoginResult.InvalidPassword:
                    mensajeError = Traducir("msgPasswordIncorrecta");
                    break;
                case LoginResult.UserBlocked:
                    mensajeError = Traducir("msgUsuarioBloqueado");
                    break;
                default:
                    mensajeError = Traducir("msgErrorLogin");
                    break;
            }
            MostrarMensaje(mensajeError, true);
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.ForeColor = esError ? System.Drawing.Color.FromName("#D93416") : System.Drawing.Color.Green;
        }
    }
}