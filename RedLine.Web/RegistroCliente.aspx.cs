using Redline.Be;
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
    public partial class RegistroCliente : BasePage
    {
        private readonly BLL_Usuario gestorUsuario = new BLL_Usuario();
        private readonly BLL_Cliente gestorCliente = new BLL_Cliente();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMensaje.Text = string.Empty;
            }
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
                lblMensaje.Text = Traducir("msg_registro_campos_obligatorios");
                return;
            }

            if (pass != passConfirm)
            {
                lblMensaje.Text = Traducir("msg_registro_pass_no_coinciden");
                return;
            }

            try
            {
                bool existeEnUsuarios = gestorUsuario.Listar().Exists(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                bool existeEnClientes = gestorCliente.ExisteEmail(email);

                if (existeEnUsuarios || existeEnClientes)
                {
                    lblMensaje.Text = Traducir("msg_registro_email_duplicado");
                    return;
                }

                Cliente nuevoCliente = new Cliente
                {
                    DNI = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    Email = email,
                    Contraseña = Hashing.Sha256(pass),
                    Telefono = telefono,
                    Direccion = direccion
                };

                gestorCliente.Insertar(nuevoCliente);

                string swalTitulo = Traducir("swal_registro_exito_titulo").Replace("'", "\\'");
                string swalTexto = Traducir("swal_registro_exito_texto").Replace("'", "\\'");
                string swalBtn = Traducir("swal_registro_exito_btn").Replace("'", "\\'");

                string script = $@"
                Swal.fire({{
                    title: '{swalTitulo}',
                    text: '{swalTexto}',
                    icon: 'success',
                    background: '#0F141C',
                    color: '#fff',
                    confirmButtonColor: '#D93416',
                    confirmButtonText: '{swalBtn}'
                }}).then((result) => {{
                    if (result.isConfirmed) {{
                        window.location.href = 'LogIn.aspx';
                    }}
                }});";

                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", script, true);
            }
            catch (Exception ex)
            {
                lblMensaje.Text = $"{Traducir("msg_registro_error_procesar")} {ex.Message}";
            }
        }
    }
}