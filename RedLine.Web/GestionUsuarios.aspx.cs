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
    public partial class GestionUsuarios : System.Web.UI.Page
    {
        private BLL_Usuario bllUsuario = new BLL_Usuario();

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!SessionManager.Instancia.IsLogged() || SessionManager.Instancia.Usuario.Rol != "Admin")
            //{
            //    Response.Redirect("Default.aspx");
            //}

            if (!IsPostBack)
            {
                CargarGrilla();
            }
        }

        private void CargarGrilla()
        {
            gvUsuarios.DataSource = bllUsuario.Listar();
            gvUsuarios.DataBind();
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["ID_EDIT"] != null)
                {
                    int id = (int)ViewState["ID_EDIT"];
                    Usuario actual = bllUsuario.ObtenerPorId(id);
                    actual.Nombre = txtNombre.Text.Trim();
                    actual.Apellido = txtApellido.Text.Trim();
                    actual.Email = txtEmail.Text.Trim();
                    actual.Rol = ddlRol.SelectedValue;
                    actual.DNI = txtDNI.Text.Trim();

                    bllUsuario.Modificar(actual);
                    lblMensaje.Text = "Usuario actualizado correctamente.";
                }
                else
                {
                    Usuario nuevo = new Usuario
                    {
                        DNI = txtDNI.Text.Trim(),
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Rol = ddlRol.SelectedValue,
                        Activo = true,
                        Bloqueado = false,
                        Intentos = 0,
                        UltimoIntento = DateTime.Now
                    };

                    bllUsuario.Insertar(nuevo);
                    lblMensaje.Text = "Usuario creado";
                }

                CargarGrilla();
                LimpiarCampos();
                lblMensaje.ForeColor = System.Drawing.Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.ForeColor = System.Drawing.Color.OrangeRed;
            }
        }

        protected void gvUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(gvUsuarios.SelectedDataKey.Value);
            Usuario u = bllUsuario.ObtenerPorId(id);

            ViewState["ID_EDIT"] = u.ID;
            txtDNI.Text = u.DNI;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            txtEmail.Text = u.Email;
            ddlRol.SelectedValue = u.Rol;

            btnAgregar.Text = "Confirmar Cambios";
            txtDNI.Enabled = false;
        }

        protected void gvUsuarios_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvUsuarios.DataKeys[e.RowIndex].Value);
            bllUsuario.Eliminar(id);
            CargarGrilla();
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CambiarEstado" || e.CommandName == "Desbloquear")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                Usuario u = bllUsuario.ObtenerPorId(id);

                if (e.CommandName == "CambiarEstado")
                {
                    if (u.Activo) bllUsuario.Desactivar(u);
                    else bllUsuario.Activar(u);
                }
                else if (e.CommandName == "Desbloquear")
                {
                    bllUsuario.DesbloquearUsuario(u);
                }

                CargarGrilla();
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtDNI.Text = txtNombre.Text = txtApellido.Text = txtEmail.Text = "";
            txtDNI.Enabled = true;
            ViewState["ID_EDIT"] = null;
            btnAgregar.Text = "Guardar Usuario";
            lblMensaje.Text = "";
        }
    }
}