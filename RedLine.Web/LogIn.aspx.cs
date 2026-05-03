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

namespace RedLine.Web
{
    public partial class LogIn : System.Web.UI.Page
    {
        BLL_Cliente gestorCliente = new BLL_Cliente();
        BLL_Usuario gestorUsuario = new BLL_Usuario();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Ingresa tus credenciales.');", true);
                return;
            }

            try
            {
                LoginResult resultado = gestorUsuario.Login(email, password); 

                if (resultado == LoginResult.ValidUser)
                { 
                    Response.Redirect("Default.aspx"); 
                    return;
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
                        Response.Redirect("Default.aspx"); 
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
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error técnico: {ex.Message}');", true); 
            }
        }

        private void ManejarErrorLogin(LoginResult resultado)
        {
            string mensajeError = "";
            switch (resultado)
            {
                case LoginResult.InvalidUsername: mensajeError = "El usuario o email no existe."; break; 
                case LoginResult.InvalidPassword: mensajeError = "Contraseña incorrecta."; break; 
                case LoginResult.UserBlocked: mensajeError = "Usuario bloqueado."; break; 
                default: mensajeError = "Error al iniciar sesión."; break; 
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{mensajeError}');", true); 
        }
    }
}