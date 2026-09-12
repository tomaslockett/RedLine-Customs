using Redline.Be;
using RedLine.Bll;
using RedLine.Servicios.Composite;
using System;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class GestionUsuarios : BasePage
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
                CargarPerfiles();
            }
        }

        private void CargarGrilla()
        {
            gvUsuarios.DataSource = bllUsuario.Listar();
            gvUsuarios.DataBind();
        }

        private void CargarPerfiles()
        {
            BLL_Perfil bllPerfil = new BLL_Perfil();
            ddlRol.DataSource = bllPerfil.Listar();
            ddlRol.DataTextField = "Nombre";
            ddlRol.DataValueField = "Id";
            ddlRol.DataBind();
        }
        protected void gvUsuarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Usuario u = (Usuario)e.Row.DataItem;

                Label lblPerfil = (Label)e.Row.FindControl("lblNombrePerfil");
                if (lblPerfil != null)
                {
                    lblPerfil.Text = (u.Perfil != null && !string.IsNullOrEmpty(u.Perfil.Nombre))
                        ? u.Perfil.Nombre
                        : Traducir("texto_sin_perfil");
                }

                LinkButton btnBorrar = (LinkButton)e.Row.FindControl("btnBorrarUsuario");
                if (btnBorrar != null)
                {
                    string confirmacion = Traducir("msg_confirmar_eliminar_usuario");
                    btnBorrar.OnClientClick = $"return confirm('{confirmacion.Replace("'", "\\'")}');";
                }
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDNI.Text) ||
                    string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtApellido.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MostrarMensaje(Traducir("msg_campos_obligatorios_usuario"), true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlRol.SelectedValue))
                {
                    MostrarMensaje(Traducir("msg_seleccionar_perfil_requerido"), true);
                    return;
                }

                BLL_Perfil bllPerfil = new BLL_Perfil();
                int idPerfilSeleccionado = int.Parse(ddlRol.SelectedValue);
                Perfil perfilSeleccionado = bllPerfil.ObtenerPorId(idPerfilSeleccionado);

                if (ViewState["ID_EDIT"] != null)
                {
                    int id = (int)ViewState["ID_EDIT"];
                    Usuario actual = bllUsuario.ObtenerPorId(id);
                    actual.Nombre = txtNombre.Text.Trim();
                    actual.Apellido = txtApellido.Text.Trim();
                    actual.Email = txtEmail.Text.Trim();
                    actual.Perfil = perfilSeleccionado;
                    actual.PerfilId = perfilSeleccionado.Id;
                    actual.DNI = txtDNI.Text.Trim();

                    bllUsuario.Modificar(actual);
                    MostrarMensaje(Traducir("msg_usuario_actualizado_exito"), false);
                }
                else
                {
                    Usuario nuevo = new Usuario
                    {
                        DNI = txtDNI.Text.Trim(),
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Contraseña = RedLine.Servicios.Hashing.Sha256("123456"),
                        Perfil = perfilSeleccionado,
                        PerfilId = perfilSeleccionado.Id,
                        Activo = true,
                        Bloqueado = false,
                        Intentos = 0,
                        UltimoIntento = DateTime.Now
                    };

                    bllUsuario.Insertar(nuevo);
                    MostrarMensaje(Traducir("msg_usuario_creado_exito"), false);
                }

                CargarGrilla();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"{Traducir("msg_error_usuario_prefijo")} {ex.Message}", true);
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

            if (u.PerfilId.HasValue)
            {
                ddlRol.SelectedValue = u.PerfilId.Value.ToString();
            }

            btnAgregar.Text = Traducir("btn_confirmar_cambios_usuario");
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
            txtDNI.Text = txtNombre.Text = txtApellido.Text = txtEmail.Text = string.Empty;
            txtDNI.Enabled = true;
            ViewState["ID_EDIT"] = null;
            btnAgregar.Text = Traducir("btnAgregar");
        }
        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.ForeColor = esError ? System.Drawing.Color.FromName("#D93416") : System.Drawing.Color.Green;
        }
    }
}