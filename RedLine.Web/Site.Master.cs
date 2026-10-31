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

        #region Traduccion
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

        #endregion

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
                RenderizarMenuCliente();
            }
            else
            {
                string textoMiCuenta = SubjectIdioma.Instancia.Traducir("litNombreUsuario_Default");
                litNombreUsuario.Text = textoMiCuenta.StartsWith("[") ? "Mi Cuenta" : textoMiCuenta;

                lnkLogin.Visible = true;
                lnkRegistrarse.Visible = true;

                lnkCambioPass.Visible = false;
                lnkLogout.Visible = false;
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

            lnkCambioPass.Visible = true;
            lnkLogout.Visible = true;
        }

        private void RenderizarMenuCliente()
        {
            menuTallerVentas.Visible = true;

            // El cliente autenticado ve Personalizar Auto y Mi Garage según su familia Seed
            lnkPersonalizarAuto.Visible = true;
            lnkMiGaraje.Visible = true;

            // Las opciones de gestión y administración quedan ocultas para el cliente
            lnkCrearAuto.Visible = false;
            lnkInventario.Visible = false;
            lnkHistorialVentas.Visible = false;
            lnkFacturacion.Visible = false; // Descarga/ve su factura directamente desde su garaje con el handler
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

                // 1. Obtenemos las raíces de permisos asignadas al perfil
                List<ComponentePermiso> permisosRaiz = bllPerfil.ObtenerPermisosDePerfil(usuario.PerfilId.Value);

                if (permisosRaiz == null || permisosRaiz.Count == 0)
                {
                    litNombreUsuario.Text = $"{usuario.Nombre} (Perfil ID: {usuario.PerfilId} sin permisos en BD)";
                    return;
                }

                // 2. Aplanamos todo el árbol usando el método nativo de tu arquitectura Composite (o recorriendo los hijos)
                List<ComponentePermiso> permisosFinales = new List<ComponentePermiso>();
                foreach (var raiz in permisosRaiz)
                {
                    if (raiz != null)
                    {
                        permisosFinales.AddRange(raiz.ObtenerHijos());
                    }
                }
                // Aseguramos que no se repitan por ID si hay cruces de familias
                permisosFinales = permisosFinales.GroupBy(p => p.Id).Select(g => g.First()).ToList();

                // ==========================================
                // 1. MENÚ TALLER Y VENTAS
                // ==========================================
                bool pInventario = permisosFinales.Any(p => p.Nombre == "GestionInventario");
                bool pPersonalizar = permisosFinales.Any(p => p.Nombre == "PersonalizarAuto");
                bool pHistorial = permisosFinales.Any(p => p.Nombre == "HistorialVentas");
                bool pMiGaraje = permisosFinales.Any(p => p.Nombre == "MiGaraje");
                bool pFacturacion = permisosFinales.Any(p => p.Nombre == "Facturacion");

                bool tieneTaller = pInventario || pPersonalizar || pHistorial || pMiGaraje || pFacturacion;
                menuTallerVentas.Visible = tieneTaller;

                if (lnkCrearAuto != null) lnkCrearAuto.Visible = pInventario;
                if (lnkInventario != null) lnkInventario.Visible = pInventario;
                if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = pPersonalizar;
                if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = pHistorial;
                if (lnkMiGaraje != null) lnkMiGaraje.Visible = pMiGaraje;
                if (lnkFacturacion != null) lnkFacturacion.Visible = pFacturacion;

                // ==========================================
                // 2. MENÚ GESTIÓN ABM
                // ==========================================
                bool pUsuarios = permisosFinales.Any(p => p.Nombre == "GestionUsuarios");
                bool pClientes = permisosFinales.Any(p => p.Nombre == "GestionClientes");
                bool pEventos = permisosFinales.Any(p => p.Nombre == "BitacoraEventos");

                bool tieneABM = pUsuarios || pClientes || pEventos;
                menuGestionABM.Visible = tieneABM;

                if (lnkGestionUsuarios != null) lnkGestionUsuarios.Visible = pUsuarios;
                if (lnkGestionClientes != null) lnkGestionClientes.Visible = pClientes;
                if (lnkGestionEventos != null) lnkGestionEventos.Visible = pEventos;

                // ==========================================
                // 3. MENÚ SISTEMA
                // ==========================================
                bool pPerfiles = permisosFinales.Any(p => p.Nombre == "GestionPerfiles");
                bool pIdiomas = permisosFinales.Any(p => p.Nombre == "GestionIdiomas");
                bool pBackup = permisosFinales.Any(p => p.Nombre == "BackupRestore");
                bool pRecuperarDV = permisosFinales.Any(p => p.Nombre == "RecuperarDV");

                bool pBackupCheck = permisosFinales.Any(p => p.Nombre == "BackupRestore");
                bool tieneSistema = pPerfiles || pIdiomas || pBackupCheck || pRecuperarDV;
                menuSistema.Visible = tieneSistema;

                if (lnkGestionPermisos != null) lnkGestionPermisos.Visible = pPerfiles;
                if (lnkGestionIdiomas != null) lnkGestionIdiomas.Visible = pIdiomas;
                if (lnkBackupRestore != null) lnkBackupRestore.Visible = pBackupCheck;
                if (lnkDigitoVerificador != null) lnkDigitoVerificador.Visible = pRecuperarDV;
            }
            catch (Exception ex)
            {
                litNombreUsuario.Text = "Error BD: " + ex.Message;
            }
        }
    }
}