using Redline.Be;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class CambioContraseña : System.Web.UI.Page
    {
        private BLL_Usuario bllUsuario = new BLL_Usuario();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SessionManager.Instancia.IsLogged())
            {
                Response.Redirect("LogIn.aspx");
            }
        }

        protected void btnCambiar_Click(object sender, EventArgs e)
        {
            try
            {
                string actual = txtPassActual.Text.Trim();
                string nueva = txtPassNueva.Text.Trim();
                string confirmar = txtPassConfirm.Text.Trim();

                if (string.IsNullOrEmpty(actual) || string.IsNullOrEmpty(nueva) || string.IsNullOrEmpty(confirmar))
                {
                    MostrarMensaje("Todos los campos son obligatorios.", true);
                    return;
                }

                if (nueva != confirmar)
                {
                    MostrarMensaje("Las nuevas contraseñas no coinciden.", true);
                    return;
                }

                Usuario usuarioActual = SessionManager.Instancia.Usuario;

                string actualHasheada = Hashing.Sha256(actual);
                if (usuarioActual.Contraseña != actualHasheada)
                {
                    MostrarMensaje("La contraseña actual es incorrecta.", true);
                    return;
                }

                string nuevaHasheada = Hashing.Sha256(nueva);

                bllUsuario.CambiarContraseñaDirecto(usuarioActual.ID, nuevaHasheada);

                BLL_Evento bllEvento = new BLL_Evento();
                bllEvento.Registrar(usuarioActual.Email, ModulosEventos.Usuarios, "Cambio de contraseña exitoso", 1);
                bllUsuario.Logout();
                Response.Redirect("LogIn.aspx");
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error: " + ex.Message, true);
            }
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.ForeColor = esError ? System.Drawing.Color.Red : System.Drawing.Color.Green;
        }
    }
 }