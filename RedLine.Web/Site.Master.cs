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

        /// <summary>
        /// Oculta por defecto los tres menús desplegables principales y cada uno de sus links internos.
        /// </summary>
        private void OcultarTodoElMenu()
        {
            menuTallerVentas.Visible = false;
            menuGestionABM.Visible = false;
            menuSistema.Visible = false;

            // Ocultamos también cada link individual de Taller y Ventas
            if (lnkCrearAuto != null) lnkCrearAuto.Visible = false;
            if (lnkPersonalizarAuto != null) lnkPersonalizarAuto.Visible = false;
            if (lnkMiGaraje != null) lnkMiGaraje.Visible = false;
            if (lnkFacturacion != null) lnkFacturacion.Visible = false;
            if (lnkInventario != null) lnkInventario.Visible = false;
            if (lnkHistorialVentas != null) lnkHistorialVentas.Visible = false;

            // Ocultamos cada link individual de Gestión ABM
            if (lnkGestionUsuarios != null) lnkGestionUsuarios.Visible = false;
            if (lnkGestionClientes != null) lnkGestionClientes.Visible = false;
            if (lnkGestionEventos != null) lnkGestionEventos.Visible = false;

            // Ocultamos cada link individual de Sistema
            if (lnkGestionPermisos != null) lnkGestionPermisos.Visible = false;
            if (lnkGestionIdiomas != null) lnkGestionIdiomas.Visible = false;
            if (lnkBackupRestore != null) lnkBackupRestore.Visible = false;
            if (lnkDigitoVerificador != null) lnkDigitoVerificador.Visible = false;
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

        /// <summary>
        /// Muestra u oculta los menús desplegables y sus links internos según los permisos
        /// asignados al perfil del usuario logueado.
        /// </summary>
        private void RenderizarMenuPorPermisos(Usuario usuario)
        {
            // Si el usuario no existe o no tiene un perfil asignado, no se muestra ningún menú
            if (usuario == null)
            {
                return;
            }

            if (!usuario.PerfilId.HasValue || usuario.PerfilId.Value <= 0)
            {
                return;
            }

            try
            {
                BLL_Perfil bllPerfil = new BLL_Perfil();

                // 1. Obtenemos los permisos o familias asignados al perfil desde la capa de negocio
                List<ComponentePermiso> permisosRaiz = bllPerfil.ObtenerPermisosDePerfil(usuario.PerfilId.Value);

                if (permisosRaiz == null || permisosRaiz.Count == 0)
                {
                    return;
                }

                // 2. Aplanamos todos los niveles del árbol de permisos de forma recursiva sin usar LINQ complejo
                List<ComponentePermiso> permisosFinales = new List<ComponentePermiso>();
                foreach (ComponentePermiso raiz in permisosRaiz)
                {
                    AgregarPermisosRecursivo(raiz, permisosFinales);
                }

                // ==========================================
                // 1. MENÚ TALLER Y VENTAS
                // ==========================================
                bool pInventario = TienePermiso(permisosFinales, "GestionInventario");
                bool pPersonalizar = TienePermiso(permisosFinales, "PersonalizarAuto");
                bool pHistorial = TienePermiso(permisosFinales, "HistorialVentas");
                bool pMiGaraje = TienePermiso(permisosFinales, "MiGaraje");
                bool pFacturacion = TienePermiso(permisosFinales, "Facturacion");

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
                bool pUsuarios = TienePermiso(permisosFinales, "GestionUsuarios");
                bool pClientes = TienePermiso(permisosFinales, "GestionClientes");
                bool pEventos = TienePermiso(permisosFinales, "BitacoraEventos");

                bool tieneABM = pUsuarios || pClientes || pEventos;
                menuGestionABM.Visible = tieneABM;

                if (lnkGestionUsuarios != null) lnkGestionUsuarios.Visible = pUsuarios;
                if (lnkGestionClientes != null) lnkGestionClientes.Visible = pClientes;
                if (lnkGestionEventos != null) lnkGestionEventos.Visible = pEventos;

                // ==========================================
                // 3. MENÚ SISTEMA
                // ==========================================
                bool pPerfiles = TienePermiso(permisosFinales, "GestionPerfiles");
                bool pIdiomas = TienePermiso(permisosFinales, "GestionIdiomas");
                bool pBackup = TienePermiso(permisosFinales, "BackupRestore");
                bool pRecuperarDV = TienePermiso(permisosFinales, "RecuperarDV");

                bool tieneSistema = pPerfiles || pIdiomas || pBackup || pRecuperarDV;
                menuSistema.Visible = tieneSistema;

                if (lnkGestionPermisos != null) lnkGestionPermisos.Visible = pPerfiles;
                if (lnkGestionIdiomas != null) lnkGestionIdiomas.Visible = pIdiomas;
                if (lnkBackupRestore != null) lnkBackupRestore.Visible = pBackup;
                if (lnkDigitoVerificador != null) lnkDigitoVerificador.Visible = pRecuperarDV;
            }
            catch (Exception)
            {
                // Ante cualquier error al consultar permisos, dejamos todos los menús ocultos
                OcultarTodoElMenu();
            }
        }

        /// <summary>
        /// Recorre de forma recursiva un componente (Permiso o Familia con sub-familias)
        /// y agrega todos los permisos hojas a la lista final evitando duplicados.
        /// </summary>
        private void AgregarPermisosRecursivo(ComponentePermiso componente, List<ComponentePermiso> listaFinal)
        {
            if (componente == null)
            {
                return;
            }

            // Si es una Familia que tiene hijos cargados en su colección, recorremos cada hijo recursivamente
            if (componente.ComponentesHijos != null && componente.ComponentesHijos.Count > 0)
            {
                foreach (ComponentePermiso hijo in componente.ComponentesHijos)
                {
                    AgregarPermisosRecursivo(hijo, listaFinal);
                }
                return;
            }

            // Obtenemos los elementos mediante ObtenerHijos() del patrón Composite
            IEnumerable<ComponentePermiso> hijos = componente.ObtenerHijos();
            if (hijos != null)
            {
                foreach (ComponentePermiso item in hijos)
                {
                    if (item != null)
                    {
                        // Si el hijo devuelto es otro componente distinto al actual, seguimos bajando recursivamente
                        if (item.Id != componente.Id || item.Nombre != componente.Nombre)
                        {
                            AgregarPermisosRecursivo(item, listaFinal);
                        }
                        else
                        {
                            // Verificamos que el permiso no esté ya en la lista antes de agregarlo
                            bool yaExiste = false;
                            foreach (ComponentePermiso existente in listaFinal)
                            {
                                if (existente.Id == item.Id && existente.Nombre == item.Nombre)
                                {
                                    yaExiste = true;
                                    break;
                                }
                            }

                            if (!yaExiste)
                            {
                                listaFinal.Add(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifica de forma simple si en la lista de permisos existe alguno con el nombre indicado.
        /// </summary>
        private bool TienePermiso(List<ComponentePermiso> listaPermisos, string nombrePermiso)
        {
            if (listaPermisos == null || string.IsNullOrEmpty(nombrePermiso))
            {
                return false;
            }

            foreach (ComponentePermiso permiso in listaPermisos)
            {
                if (permiso != null && !string.IsNullOrEmpty(permiso.Nombre))
                {
                    if (string.Equals(permiso.Nombre.Trim(), nombrePermiso, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}