using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Be.Interfaces;
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
    public partial class SiteMaster : MasterPage, IObserver
    {
        private readonly BLL_Idioma _bllIdioma = new BLL_Idioma();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SubjectIdioma.Instancia.AgregarObserver(this);
        }

        protected override void OnUnload(EventArgs e)
        {
            SubjectIdioma.Instancia.QuitarObserver(this);
            base.OnUnload(e);
        }

        public void ActualizarIdioma(string nuevoIdioma)
        {
            TraducirControles(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            TraducirControles(this);
        }

        private void TraducirControles(Control controlPadre)
        {
            foreach (Control c in controlPadre.Controls)
            {
                if (c is Label lbl && !string.IsNullOrEmpty(lbl.ID))
                {
                    string texto = SubjectIdioma.Instancia.Traducir(lbl.ID);
                    if (!texto.StartsWith("[") || !texto.EndsWith("]"))
                    {
                        lbl.Text = texto;
                    }
                }
                else if (c is Button btn && !string.IsNullOrEmpty(btn.ID))
                {
                    string texto = SubjectIdioma.Instancia.Traducir(btn.ID);
                    if (!texto.StartsWith("[") || !texto.EndsWith("]"))
                    {
                        btn.Text = texto;
                    }
                }

                if (c.HasControls())
                {
                    TraducirControles(c);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarComboIdiomas();
            }

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

                // Cliente tiene acceso al menú Taller y Ventas (Personalizar Auto, Mis Vehículos y Facturación)
                menuTallerVentas.Visible = true;
                if (lnkCrearAuto != null) lnkCrearAuto.Visible = false;
                if (lnkInventario != null) lnkInventario.Visible = false;
                if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = false;
                if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = true;
                if (lnkTallerMisAutos != null) lnkTallerMisAutos.Visible = true;
                if (lnkTallerFacturacion != null) lnkTallerFacturacion.Visible = true;
            }
            else
            {
                string textoMiCuenta = SubjectIdioma.Instancia.Traducir("litNombreUsuario_Default");
                litNombreUsuario.Text = textoMiCuenta.StartsWith("[") ? "Mi Cuenta" : textoMiCuenta;

                lnkLogin.Visible = true;
                lnkRegistrarse.Visible = true;
                lnkCambioPass.Visible = false;
                lnkLogout.Visible = false;

                // Acceso para clientes exploradores y visitantes a Taller y Ventas
                menuTallerVentas.Visible = true;
                if (lnkCrearAuto != null) lnkCrearAuto.Visible = false;
                if (lnkInventario != null) lnkInventario.Visible = false;
                if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = false;
                if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = true;
                if (lnkTallerMisAutos != null) lnkTallerMisAutos.Visible = true;
                if (lnkTallerFacturacion != null) lnkTallerFacturacion.Visible = true;
            }
        }

        private void CargarComboIdiomas()
        {
            List<Idioma> idiomas = _bllIdioma.Listar();
            ddlIdioma.DataSource = idiomas;
            ddlIdioma.DataTextField = "Nombre";
            ddlIdioma.DataValueField = "ID";
            ddlIdioma.DataBind();

            Idioma seleccionado = null;

            if (Session["IdiomaSeleccionado"] != null)
            {
                int idSesion = (int)Session["IdiomaSeleccionado"];
                seleccionado = idiomas.FirstOrDefault(i => i.ID == idSesion);
            }

            if (seleccionado == null)
            {
                seleccionado = _bllIdioma.ObtenerDefault() ?? idiomas.FirstOrDefault();
            }

            if (seleccionado != null)
            {
                ddlIdioma.SelectedValue = seleccionado.ID.ToString();
                Session["IdiomaSeleccionado"] = seleccionado.ID;
                ActualizarSubjectIdioma(seleccionado);
            }
        }

        protected void DdlIdioma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlIdioma.SelectedValue)) return;

            int idIdioma = Convert.ToInt32(ddlIdioma.SelectedValue);
            Idioma idiomaSeleccionado = _bllIdioma.ObtenerPorId(idIdioma);

            if (idiomaSeleccionado != null)
            {
                Session["IdiomaSeleccionado"] = idiomaSeleccionado.ID;
                ActualizarSubjectIdioma(idiomaSeleccionado);
                if (SessionManager.Instancia.IsLogged())
                {
                    BLL_Usuario bllUsuario = new BLL_Usuario();
                    bllUsuario.ActualizarIdiomaUsuario(SessionManager.Instancia.Usuario.ID, idiomaSeleccionado.Nombre);
                }
                Response.Redirect(Request.RawUrl);
            }
        }

        private void ActualizarSubjectIdioma(Idioma idioma)
        {
            var traducciones = _bllIdioma.ObtenerTraduccionesPorIdioma(idioma.ID);
            SubjectIdioma.Instancia.CargarTraducciones(idioma.Nombre, traducciones);
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
            lnkCambioPass.Visible = SessionManager.Instancia.IsLogged();
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
                menuTallerVentas.Visible = true;
                if (lnkCrearAuto != null) lnkCrearAuto.Visible = false;
                if (lnkInventario != null) lnkInventario.Visible = false;
                if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = false;
                if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = true;
                if (lnkTallerMisAutos != null) lnkTallerMisAutos.Visible = true;
                if (lnkTallerFacturacion != null) lnkTallerFacturacion.Visible = true;
                return;
            }

            try
            {
                BLL_Perfil bllPerfil = new BLL_Perfil();
                List<ComponentePermiso> permisosUsuario = bllPerfil.ObtenerPermisosDePerfil(usuario.PerfilId.Value);

                if (permisosUsuario == null || permisosUsuario.Count == 0)
                {
                    litNombreUsuario.Text = $"{usuario.Nombre} (Perfil ID: {usuario.PerfilId} sin permisos en BD)";
                    menuTallerVentas.Visible = true;
                    if (lnkCrearAuto != null) lnkCrearAuto.Visible = false;
                    if (lnkInventario != null) lnkInventario.Visible = false;
                    if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = false;
                    if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = true;
                    if (lnkTallerMisAutos != null) lnkTallerMisAutos.Visible = true;
                    if (lnkTallerFacturacion != null) lnkTallerFacturacion.Visible = true;
                    return;
                }

                // Taller y Ventas siempre habilitado para el usuario
                menuTallerVentas.Visible = true;

                bool tieneInventario = permisosUsuario.Any(p => p.Nombre == "GestionInventario");
                if (lnkCrearAuto != null) lnkCrearAuto.Visible = tieneInventario;
                if (lnkInventario != null) lnkInventario.Visible = tieneInventario;

                bool tieneHistorial = permisosUsuario.Any(p => p.Nombre == "HistorialVentas");
                if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = tieneHistorial;

                if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = true;
                if (lnkTallerMisAutos != null) lnkTallerMisAutos.Visible = true;
                if (lnkTallerFacturacion != null) lnkTallerFacturacion.Visible = true;

                bool tieneABM = permisosUsuario.Any(p => p.Nombre == "GestionUsuarios" || p.Nombre == "GestionClientes" || p.Nombre == "BitacoraEventos");
                menuGestionABM.Visible = tieneABM;

                lnkGestionUsuarios.Visible = permisosUsuario.Any(p => p.Nombre == "GestionUsuarios");
                lnkGestionClientes.Visible = permisosUsuario.Any(p => p.Nombre == "GestionClientes");
                lnkGestionEventos.Visible = permisosUsuario.Any(p => p.Nombre == "BitacoraEventos");

                bool tieneSistema = permisosUsuario.Any(p => p.Nombre == "GestionPerfiles" || p.Nombre == "GestionIdiomas" || p.Nombre == "BackupRestore" || p.Nombre == "RecuperarDV");
                menuSistema.Visible = tieneSistema;

                lnkGestionPermisos.Visible = permisosUsuario.Any(p => p.Nombre == "GestionPerfiles");
                lnkGestionIdiomas.Visible = permisosUsuario.Any(p => p.Nombre == "GestionIdiomas");
                lnkBackupRestore.Visible = permisosUsuario.Any(p => p.Nombre == "BackupRestore");
                lnkDigitoVerificador.Visible = permisosUsuario.Any(p => p.Nombre == "RecuperarDV");
            }
            catch (Exception ex)
            {
                litNombreUsuario.Text = "Error BD: " + ex.Message;
                menuTallerVentas.Visible = true;
                if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = true;
                if (lnkTallerMisAutos != null) lnkTallerMisAutos.Visible = true;
                if (lnkTallerFacturacion != null) lnkTallerFacturacion.Visible = true;
            }
        }
    }
}
