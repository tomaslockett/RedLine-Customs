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
    public partial class RegistroUsuario : System.Web.UI.Page
    {
        BLL_Usuario gestorUsuario = new BLL_Usuario();
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMensaje.Text = "";
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string direccion = txtDireccion.Text.Trim();
            string pass = txtPassword.Text;
            string passConfirm = txtPasswordConfirm.Text;

            if (string.IsNullOrEmpty(dni) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                lblMensaje.Text = "Los campos principales son obligatorios.";
                return;
            }

            if (pass != passConfirm)
            {
                lblMensaje.Text = "Las contraseñas no coinciden.";
                return;
            }

            try
            {
                Usuario nuevoUsuario = new Usuario();
                nuevoUsuario.Email = email;
                nuevoUsuario.Contraseña = pass;
                nuevoUsuario.Rol = "Cliente";
                nuevoUsuario.Activo = true;
                nuevoUsuario.Bloqueado = false;
                nuevoUsuario.Intentos = 0;
                nuevoUsuario.UltimoIntento = DateTime.Now;

                gestorUsuario.Insertar(nuevoUsuario);

                string script = @"
                Swal.fire({
                    title: '¡Bienvenido!',
                    text: 'Tu cuenta fue creada con éxito.',
                    icon: 'success',
                    background: '#0F141C',
                    color: '#fff',
                    confirmButtonColor: '#D93416',
                    confirmButtonText: 'Ir al Login'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = 'LogIn.aspx';
                    }
                });";
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", script, true);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate"))
                {
                    lblMensaje.Text = "El email ya está registrado.";
                }
                else
                {
                    lblMensaje.Text = "Error técnico: " + ex.Message;
                }
            }
        }
    }
}