using RedLine.Bll;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class Gestion_Perfiles_Permisos : BasePage
    {
        BLL_Perfil bllPerfil = new BLL_Perfil();
        BLL_Permisos bllPermisos = new BLL_Permisos();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPerfiles();
                CargarPermisos();
            }
        }

        private void CargarPermisos()
        {
            cblPermisosCatalogo.Items.Clear();
            cblPermisosVentas.Items.Clear();
            cblPermisosAuditoria.Items.Clear();

            var todosLosPermisos = bllPermisos.Listar().Where(p => p is Permiso).ToList();

            foreach (var permiso in todosLosPermisos)
            {
                ListItem item = new ListItem(permiso.Nombre, permiso.Id.ToString());

                switch (permiso.Id)
                {
                    case 3: // Catalogo
                    case 5: // GestionInventario
                    case 16: // PersonalizarAuto
                        cblPermisosCatalogo.Items.Add(item);
                        break;

                    case 4: // RealizarCompra
                    case 6: // GestionClientes
                    case 12: // Checkout
                    case 14: // HistorialVentas
                    case 15: // PagoExitoso
                    case 18: // RegistroClientes
                        cblPermisosVentas.Items.Add(item);
                        break;

                    case 1: // Login
                    case 2: // Logout
                    case 7: // BitacoraEventos
                    case 8: // GestionUsuarios
                    case 9: // GestionSeguridad
                    case 10: // BackupRestore
                    case 11: // CambioContraseña
                    case 13: // GestionPerfiles
                    case 17: // RecuperarDV
                    case 19: // GestionIdiomas
                        cblPermisosAuditoria.Items.Add(item);
                        break;
                }
            }
        }
        

        private void CargarPerfiles()
        {
            var perfiles = bllPerfil.Listar();
            lstPerfiles.DataSource = perfiles;
            lstPerfiles.DataTextField = "Nombre";
            lstPerfiles.DataValueField = "Id";
            lstPerfiles.DataBind();
        }

        protected void btnCrearPerfil_Click(object sender, EventArgs e)
        {
           try
            {
                string nuevoPerfil = txtNuevoPerfil.Text.Trim();

                if (string.IsNullOrEmpty(nuevoPerfil))
                {
                    MostrarAlertaTraducida("msg_ingrese_nombre_perfil");
                    return;
                }

                Perfil p = new Perfil { Nombre = nuevoPerfil };
                bllPerfil.Insertar(p);
                bllPerfil.RecalcularIntegridad();

                txtNuevoPerfil.Text = string.Empty;
                CargarPerfiles();

                MostrarAlertaTraducida("msg_perfil_creado_exito");
            }
            catch (Exception ex)
            {
                MostrarAlertaTraducida("msg_error_crear_perfil", ex.Message);
            }
        }

        protected void lstPerfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lstPerfiles.SelectedValue)) return;

            int idPerfil = int.Parse(lstPerfiles.SelectedValue);
            lblPerfilSeleccionado.Text = lstPerfiles.SelectedItem.Text;

            LimpiarCheckBoxes();

            var permisosAsignados = bllPerfil.ObtenerPermisosDePerfil(idPerfil);

            foreach (var permiso in permisosAsignados)
            {
                MarcarPermisoEnLista(cblPermisosCatalogo, permiso.Id);
                MarcarPermisoEnLista(cblPermisosVentas, permiso.Id);
                MarcarPermisoEnLista(cblPermisosAuditoria, permiso.Id);
            }

        }

        private void LimpiarCheckBoxes()
        {
            foreach (ListItem item in cblPermisosCatalogo.Items) item.Selected = false;
            foreach (ListItem item in cblPermisosVentas.Items) item.Selected = false;
            foreach (ListItem item in cblPermisosAuditoria.Items) item.Selected = false;
        }

        private void MarcarPermisoEnLista(CheckBoxList cbl, int id)
        {
            foreach (ListItem item in cbl.Items)
            {
                if (item.Value == id.ToString()) item.Selected = true;
            }
        }


        protected void btnDescartar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Catalogo.aspx");
        }

        protected void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lstPerfiles.SelectedValue)) return;

            try
            {
                int idPerfil = int.Parse(lstPerfiles.SelectedValue);
                List<int> idsSeleccionados = new List<int>();

                RecolectarSeleccionados(cblPermisosCatalogo, idsSeleccionados);
                RecolectarSeleccionados(cblPermisosVentas, idsSeleccionados);
                RecolectarSeleccionados(cblPermisosAuditoria, idsSeleccionados);

                bllPerfil.SincronizarPermisos(idPerfil, idsSeleccionados);
                bllPerfil.RecalcularIntegridad();

                MostrarAlertaTraducida("msg_cambios_guardados_exito");
            }
            catch (Exception ex)
            {
                MostrarAlertaTraducida("msg_error_general", ex.Message);
            }
        }

        private void RecolectarSeleccionados(CheckBoxList cbl, List<int> lista)
        {
            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected)
                {
                    lista.Add(int.Parse(item.Value));
                }
            }
        }
    }
}