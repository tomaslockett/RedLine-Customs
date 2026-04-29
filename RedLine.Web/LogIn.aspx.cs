using Redline.Be;
using RedLine.Bll;
using RedLine.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class LogIn : System.Web.UI.Page
    {
        BLL_Usuario gestorUsuario = new BLL_Usuario();
        protected void Page_Load(object sender, EventArgs e)
        {
            //Usuario nuevoAdmin = new Usuario();
            //nuevoAdmin.Username = "admin";
            //nuevoAdmin.Contraseña = "123"; 
            //nuevoAdmin.Rol = "Admin";
            //nuevoAdmin.UltimoIntento = DateTime.Now;
            //gestorUsuario.Insertar(nuevoAdmin);
            btnLogin.Click += BtnLogin_Click;
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Ingresa tus credenciales.');", true);
                return;
            }

            try
            {
                LoginResult resultado = gestorUsuario.Login(username, password);

                if (resultado == LoginResult.ValidUser)
                {
                    Response.Redirect("Default.aspx");
                }
            }
            catch (LoginException ex)
            {
                string mensajeError = "";
                switch (ex.Result)
                {
                    case LoginResult.InvalidUsername: mensajeError = "El usuario no existe."; break;
                    case LoginResult.InvalidPassword: mensajeError = "Contraseña incorrecta."; break;
                    case LoginResult.UserBlocked: mensajeError = "Usuario bloqueado temporalmente. " + ex.Message; break;
                    default: mensajeError = "Error al iniciar sesión."; break;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{mensajeError}');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error técnico: {ex.Message}');", true);
            }
        }
    }
}