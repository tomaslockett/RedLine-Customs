using Redline.Be;
using RedLine.Bll;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OcultarTodoElMenu();

            if (SessionManager.Instancia.IsLogged())
            {
                Usuario userBackend = SessionManager.Instancia.Usuario;
                litNombreUsuario.Text = $"{userBackend.Nombre} {userBackend.Apellido}";

                EstablecerMenuLogueado();
                RenderizarMenuPorPermisos(userBackend);
            }
            else if (Session["ClienteSession"] != null)
            {
                var clienteLogueado = (Cliente)Session["ClienteSession"];
                litNombreUsuario.Text = $"{clienteLogueado.Nombre} {clienteLogueado.Apellido}";

                EstablecerMenuLogueado();
                menuTallerVentas.Visible = true;
            }
            else
            {
                litNombreUsuario.Text = "Mi Cuenta";

                lnkLogin.Visible = true;
                lnkRegistrarse.Visible = true;

                lnkCambioPass.Visible = false;
                lnkLogout.Visible = false;
            }
        }

        private void OcultarTodoElMenu()
        {
            menuTallerVentas.Visible = false;
            menuGestionABM.Visible = false;
            menuSistema.Visible = false;
        }

        private void EstablecerMenuLogueado()
        {
            lnkLogin.Visible = false;
            lnkRegistrarse.Visible = false;

            lnkCambioPass.Visible = true;
            lnkLogout.Visible = true;
        }

        private void RenderizarMenuPorPermisos(Usuario usuario)
        {
            if (usuario == null)
            {
                litNombreUsuario.Text = "ERROR: Usuario es Null";
                return;
            }

            if (!usuario.PerfilId.HasValue || usuario.PerfilId.Value <= 0)
            {
                litNombreUsuario.Text = $"{usuario.Nombre} (Sin Perfil ID asignado)";
                return;
            }

            try
            {
                BLL_Perfil bllPerfil = new BLL_Perfil();
                List<ComponentePermiso> permisosUsuario = bllPerfil.ObtenerPermisosDePerfil(usuario.PerfilId.Value);

                if (permisosUsuario == null || permisosUsuario.Count == 0)
                {
                    litNombreUsuario.Text = $"{usuario.Nombre} (Perfil ID: {usuario.PerfilId} sin permisos en BD)";
                    return;
                }

                bool tieneAccesoTaller = permisosUsuario.Any(p => p.Nombre == "RealizarCompra" || p.Nombre == "GestionInventario" || p.Nombre == "Catalogo");
                menuTallerVentas.Visible = tieneAccesoTaller;
                lnkInventario.Visible = permisosUsuario.Any(p => p.Nombre == "GestionInventario");

                bool tieneAccesoABM = permisosUsuario.Any(p => p.Nombre == "GestionUsuarios" || p.Nombre == "GestionClientes" || p.Nombre == "BitacoraEventos");
                menuGestionABM.Visible = tieneAccesoABM;

                lnkGestionUsuarios.Visible = permisosUsuario.Any(p => p.Nombre == "GestionUsuarios");
                lnkGestionClientes.Visible = permisosUsuario.Any(p => p.Nombre == "GestionClientes");
                lnkGestionEventos.Visible = permisosUsuario.Any(p => p.Nombre == "BitacoraEventos");

                bool tieneAccesoSistema = permisosUsuario.Any(p => p.Nombre == "GestionSeguridad");
                menuSistema.Visible = tieneAccesoSistema;

                lnkGestionPermisos.Visible = permisosUsuario.Any(p => p.Nombre == "GestionSeguridad");
                lnkBackupRestore.Visible = permisosUsuario.Any(p => p.Nombre == "GestionSeguridad");
                lnkDigitoVerificador.Visible = permisosUsuario.Any(p => p.Nombre == "GestionSeguridad");
            }
            catch (Exception ex)
            {
                litNombreUsuario.Text = "Error BD: " + ex.Message;
            }
        }
    }
}