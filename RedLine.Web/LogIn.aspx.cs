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
    public partial class LogIn : System.Web.UI.Page, IObserver
    {
        BLL_Cliente gestorCliente = new BLL_Cliente();
        BLL_Usuario gestorUsuario = new BLL_Usuario();

        protected void Page_Load(object sender, EventArgs e)
        {
            SubjectIdioma.Instancia.AgregarObserver(this);

            if (!IsPostBack)
            {
                ActualizarIdioma(SubjectIdioma.Instancia.IdiomaActual);
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            SubjectIdioma.Instancia.QuitarObserver(this);
        }

        public void ActualizarIdioma(string nuevoIdioma)
        {
            lblTitulo.Text = SubjectIdioma.Instancia.Traducir("lblTitulo");
            lblEmail.Text = SubjectIdioma.Instancia.Traducir("lblEmail");
            lblPassword.Text = SubjectIdioma.Instancia.Traducir("lblPassword");
            btnLogin.Text = SubjectIdioma.Instancia.Traducir("btnLogin");
            lblNoTienesCuenta.Text = SubjectIdioma.Instancia.Traducir("lblNoTienesCuenta");
            linkRegistro.Text = SubjectIdioma.Instancia.Traducir("linkRegistro");
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MostrarMensaje(SubjectIdioma.Instancia.Traducir("msgIngresaCredenciales"), true);
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
                    MostrarMensaje(SubjectIdioma.Instancia.Traducir("msgSistemaNoFunciona"), true);
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
                string textoError = SubjectIdioma.Instancia.Traducir("msgErrorTecnico");
                MostrarMensaje($"{textoError}: {ex.Message}", true);
            }
        }

        private void ManejarErrorLogin(LoginResult resultado)
        {
            string mensajeError = "";
            switch (resultado)
            {
                case LoginResult.InvalidUsername:
                    mensajeError = SubjectIdioma.Instancia.Traducir("msgUsuarioNoExiste");
                    break;
                case LoginResult.InvalidPassword:
                    mensajeError = SubjectIdioma.Instancia.Traducir("msgPasswordIncorrecta");
                    break;
                case LoginResult.UserBlocked:
                    mensajeError = SubjectIdioma.Instancia.Traducir("msgUsuarioBloqueado");
                    break;
                default:
                    mensajeError = SubjectIdioma.Instancia.Traducir("msgErrorLogin");
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