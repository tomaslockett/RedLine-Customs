using RedLine.Bll;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace RedLine.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Se ejecuta en cada petición cuando el estado de sesión ya está disponible.
        /// Valida el estado de integridad (DV), si hay un usuario o cliente con sesión activa
        /// y los permisos necesarios para acceder a cada página, evitando bucles de redirección.
        /// </summary>
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            if (context.Session != null)
            {
                string rutaActual = context.Request.AppRelativeCurrentExecutionFilePath.ToLower();

                // Se comparan los nombres sin ".aspx" para soportar FriendlyUrls (ej: ~/LogIn y ~/Catalogo)
                bool esPermitidaEnInconsistencia = rutaActual.Contains("recuperaciondv") ||
                                                   rutaActual.Contains("backuprestore") ||
                                                   rutaActual.Contains("login") ||
                                                   rutaActual.Contains("logout");

                if (context.Session["Inconsistencia"] != null && (bool)context.Session["Inconsistencia"])
                {
                    if (!esPermitidaEnInconsistencia)
                    {
                        context.Session["Inconsistencia"] = false;
                        SessionManager.Instancia.Logout();
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    if (rutaActual.Contains("recuperaciondv") || rutaActual.Contains("backuprestore"))
                    {
                        bool tienePermisoSeguridad = false;

                        if (SessionManager.Instancia.IsLogged())
                        {
                            int idPerfilUsuario = (int)SessionManager.Instancia.Usuario.PerfilId;
                            var gestorPerfil = new BLL_Perfil();
                            List<ComponentePermiso> permisosUsuario = gestorPerfil.ObtenerPermisosDePerfil(idPerfilUsuario);

                            foreach (ComponentePermiso permiso in permisosUsuario)
                            {
                                if (permiso.Nombre.ToLower() == "gestionseguridad")
                                {
                                    tienePermisoSeguridad = true;
                                    break;
                                }
                            }

                            // Respaldo para que el perfil WebMaster siempre pueda recuperar el sistema
                            if (!tienePermisoSeguridad && SessionManager.Instancia.Usuario.Perfil != null)
                            {
                                if (SessionManager.Instancia.Usuario.Perfil.Nombre == "WebMaster")
                                {
                                    tienePermisoSeguridad = true;
                                }
                            }
                        }

                        if (!tienePermisoSeguridad)
                        {
                            context.Session["Inconsistencia"] = false;
                            SessionManager.Instancia.Logout();
                            context.Response.Redirect("~/LogIn.aspx", false);
                            context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                    }

                    return;
                }

                // Páginas públicas a las que se puede entrar sin iniciar sesión
                bool esPaginaPublica = rutaActual.Contains("login") || rutaActual.Contains("registrocliente");

                // Verificamos si hay un Usuario del sistema o un Cliente autenticado en sesión
                bool usuarioLogueado = SessionManager.Instancia.IsLogged();
                bool clienteLogueado = context.Session["ClienteSession"] != null;

                // Si no hay ni usuario ni cliente logueado, redirigimos al Login
                if (!usuarioLogueado && !clienteLogueado)
                {
                    if (!esPaginaPublica)
                    {
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                    }
                    return;
                }

                // Si ingresó un Cliente (guardado en Session["ClienteSession"]), validamos sus páginas permitidas
                if (clienteLogueado && !usuarioLogueado)
                {
                    bool permitidaParaCliente = EsPaginaPermitidaParaCliente(rutaActual);

                    if (!permitidaParaCliente)
                    {
                        context.Response.Redirect("~/Catalogo.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                    }
                    return;
                }

                // Si ingresó un Usuario del sistema, validamos su permiso según su perfil
                if (usuarioLogueado)
                {
                    string permisoRequerido = ObtenerPermisoRequeridoPorPagina(rutaActual);

                    if (permisoRequerido != "libre")
                    {
                        int idPerfilUsuario = (int)SessionManager.Instancia.Usuario.PerfilId;
                        var gestorPerfil = new BLL_Perfil();
                        List<ComponentePermiso> permisosUsuario = gestorPerfil.ObtenerPermisosDePerfil(idPerfilUsuario);

                        bool tienePermiso = false;
                        foreach (ComponentePermiso permiso in permisosUsuario)
                        {
                            if (permiso.Nombre.ToLower() == permisoRequerido.ToLower())
                            {
                                tienePermiso = true;
                                break;
                            }
                        }

                        if (!tienePermiso)
                        {
                            context.Response.Redirect("~/Catalogo.aspx", false);
                            context.ApplicationInstance.CompleteRequest();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determina si una ruta puede ser accedida por un cliente autenticado.
        /// </summary>
        private bool EsPaginaPermitidaParaCliente(string rutaActual)
        {
            if (rutaActual == "~/" ||
                rutaActual.Contains("catalogo") ||
                rutaActual.Contains("personalizarauto") ||
                rutaActual.Contains("checkout") ||
                rutaActual.Contains("pagoexitoso") ||
                rutaActual.Contains("migaraje") ||
                rutaActual.Contains("descargarfactura") ||
                rutaActual.Contains("cambiocontraseña") ||
                rutaActual.Contains("logout") ||
                rutaActual.Contains("login") ||
                rutaActual.Contains("registrocliente"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Devuelve el nombre del permiso necesario para acceder a cada página,
        /// o "libre" si es accesible para cualquier usuario que haya iniciado sesión.
        /// </summary>
        private string ObtenerPermisoRequeridoPorPagina(string rutaActual)
        {
            // El catálogo y las páginas de acceso básico son libres para cualquier usuario logueado.
            // Esto además evita que una redirección a Catalogo.aspx vuelva a redirigirse a sí misma.
            if (rutaActual == "~/" ||
                rutaActual.Contains("catalogo") ||
                rutaActual.Contains("login") ||
                rutaActual.Contains("registrocliente") ||
                rutaActual.Contains("logout") ||
                rutaActual.Contains("descargarfactura"))
            {
                return "libre";
            }

            // Taller y Ventas
            if (rutaActual.Contains("crearauto")) return "GestionInventario";
            if (rutaActual.Contains("personalizarauto")) return "PersonalizarAuto";
            if (rutaActual.Contains("inventario")) return "GestionInventario";
            if (rutaActual.Contains("historialventas")) return "HistorialVentas";
            if (rutaActual.Contains("migaraje")) return "MiGaraje";
            if (rutaActual.Contains("facturacion")) return "Facturacion";

            // Flujo de Compras
            if (rutaActual.Contains("checkout")) return "Checkout";
            if (rutaActual.Contains("pagoexitoso")) return "PagoExitoso";

            // Gestión ABM
            if (rutaActual.Contains("gestionusuarios")) return "GestionUsuarios";
            if (rutaActual.Contains("gestionclientes")) return "GestionClientes";
            if (rutaActual.Contains("gestioneventos")) return "BitacoraEventos";

            // Sistema
            if (rutaActual.Contains("gestion_perfiles_permisos")) return "GestionPerfiles";
            if (rutaActual.Contains("backuprestore")) return "BackupRestore";
            if (rutaActual.Contains("recuperaciondv") || rutaActual.Contains("digitoverificador")) return "RecuperarDV";
            if (rutaActual.Contains("cambiocontraseña")) return "CambioContraseña";
            if (rutaActual.Contains("gestionidiomas")) return "GestionIdiomas";

            // Cierre por defecto
            return "bloqueado_por_defecto";
        }

    }
}