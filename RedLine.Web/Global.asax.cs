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

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            if (context.Session != null)
            {
                string rutaActual = (context.Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLower();

                // 1. Permitir archivos estáticos, recursos y bundles sin validación de sesión
                if (rutaActual.EndsWith(".css") || rutaActual.EndsWith(".js") || rutaActual.EndsWith(".png") ||
                    rutaActual.EndsWith(".jpg") || rutaActual.EndsWith(".jpeg") || rutaActual.EndsWith(".jfif") ||
                    rutaActual.EndsWith(".ico") || rutaActual.EndsWith(".axd") || rutaActual.EndsWith(".woff") ||
                    rutaActual.EndsWith(".woff2") || rutaActual.Contains("/bundles/"))
                {
                    return;
                }

                // 2. Control de Inconsistencia de Dígito Verificador (DV)
                bool esPermitidaEnInconsistencia = rutaActual.Contains("recuperaciondv.aspx") ||
                                                   rutaActual.Contains("backuprestore.aspx") ||
                                                   rutaActual.Contains("login.aspx") ||
                                                   rutaActual.Contains("logout.aspx");

                if (context.Session["Inconsistencia"] != null && (bool)context.Session["Inconsistencia"])
                {
                    if (!esPermitidaEnInconsistencia)
                    {
                        SessionManager.Instancia.Logout();
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    if (rutaActual.Contains("recuperaciondv.aspx") || rutaActual.Contains("backuprestore.aspx"))
                    {
                        bool tienePermisoSeguridad = false;

                        if (SessionManager.Instancia.IsLogged() && SessionManager.Instancia.Usuario?.PerfilId != null)
                        {
                            try
                            {
                                int idPerfilUsuario = SessionManager.Instancia.Usuario.PerfilId.Value;
                                var gestorPerfil = new BLL_Perfil();
                                List<ComponentePermiso> permisosUsuario = gestorPerfil.ObtenerPermisosDePerfil(idPerfilUsuario);

                                tienePermisoSeguridad = permisosUsuario != null &&
                                    permisosUsuario.Any(p => p.Nombre.ToLower() == "gestionseguridad");
                            }
                            catch { }
                        }

                        if (!tienePermisoSeguridad)
                        {
                            SessionManager.Instancia.Logout();
                            context.Response.Redirect("~/LogIn.aspx", false);
                            context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                    }

                    return;
                }

                // 3. Estado de autenticación
                bool esUsuarioLogueado = SessionManager.Instancia.IsLogged();
                bool esClienteLogueado = context.Session["ClienteSession"] != null;

                // Páginas públicas y operativas accesibles tanto para visitantes como para clientes
                bool esPaginaPublicaOCliente = rutaActual.Contains("login") ||
                                               rutaActual.Contains("registrocliente") ||
                                               rutaActual.Contains("logout") ||
                                               rutaActual.Contains("catalogo") ||
                                               rutaActual.Contains("personalizarauto") ||
                                               rutaActual.Contains("misautos") ||
                                               rutaActual.Contains("facturacion") ||
                                               rutaActual.Contains("checkout") ||
                                               rutaActual.Contains("pagoexitoso") ||
                                               rutaActual.Contains("cambiocontrase") ||
                                               rutaActual == "~/" ||
                                               rutaActual == "~/default.aspx";

                // Si no hay ninguna sesión iniciada (visitante anónimo)
                if (!esUsuarioLogueado && !esClienteLogueado)
                {
                    if (!esPaginaPublicaOCliente)
                    {
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                    }
                    return;
                }

                // Si es un cliente autenticado (sin rol interno de gestión administrativa)
                if (esClienteLogueado && !esUsuarioLogueado)
                {
                    if (!esPaginaPublicaOCliente)
                    {
                        context.Response.Redirect("~/Catalogo.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                    }
                    return;
                }

                // Si es un usuario interno / backoffice (WebMaster, Administrador, etc.)
                if (esUsuarioLogueado)
                {
                    string permisoRequerido = ObtenerPermisoRequeridoPorPagina(rutaActual);

                    if (permisoRequerido != "libre")
                    {
                        var usuario = SessionManager.Instancia.Usuario;
                        if (usuario?.PerfilId == null)
                        {
                            if (!rutaActual.Contains("catalogo"))
                            {
                                context.Response.Redirect("~/Catalogo.aspx", false);
                                context.ApplicationInstance.CompleteRequest();
                            }
                            return;
                        }

                        try
                        {
                            var gestorPerfil = new BLL_Perfil();
                            List<ComponentePermiso> permisosUsuario = gestorPerfil.ObtenerPermisosDePerfil(usuario.PerfilId.Value);

                            bool tienePermiso = permisosUsuario != null &&
                                permisosUsuario.Any(p => p.Nombre.Equals(permisoRequerido, StringComparison.OrdinalIgnoreCase));

                            if (!tienePermiso)
                            {
                                if (!rutaActual.Contains("catalogo"))
                                {
                                    context.Response.Redirect("~/Catalogo.aspx", false);
                                    context.ApplicationInstance.CompleteRequest();
                                }
                            }
                        }
                        catch
                        {
                            if (!rutaActual.Contains("catalogo"))
                            {
                                context.Response.Redirect("~/Catalogo.aspx", false);
                                context.ApplicationInstance.CompleteRequest();
                            }
                        }
                    }
                }
            }
        }

        private string ObtenerPermisoRequeridoPorPagina(string rutaActual)
        {
            if (rutaActual.Contains("login") ||
                rutaActual.Contains("registrocliente") ||
                rutaActual.Contains("logout") ||
                rutaActual.Contains("catalogo") ||
                rutaActual.Contains("personalizarauto") ||
                rutaActual.Contains("misautos") ||
                rutaActual.Contains("facturacion") ||
                rutaActual.Contains("checkout") ||
                rutaActual.Contains("pagoexitoso") ||
                rutaActual.Contains("cambiocontrase") ||
                rutaActual == "~/" ||
                rutaActual == "~/default.aspx")
            {
                return "libre";
            }

            // Taller y Ventas - Backoffice
            if (rutaActual.Contains("crearauto")) return "GestionInventario";
            if (rutaActual.Contains("inventario")) return "GestionInventario";
            if (rutaActual.Contains("historialventas")) return "HistorialVentas";

            // Gestión ABM
            if (rutaActual.Contains("gestionusuarios")) return "GestionUsuarios";
            if (rutaActual.Contains("gestionclientes")) return "GestionClientes";
            if (rutaActual.Contains("gestioneventos")) return "BitacoraEventos";

            // Sistema
            if (rutaActual.Contains("gestion_perfiles_permisos")) return "GestionPerfiles";
            if (rutaActual.Contains("backuprestore")) return "BackupRestore";
            if (rutaActual.Contains("recuperaciondv") || rutaActual.Contains("digitoverificador")) return "RecuperarDV";
            if (rutaActual.Contains("gestionidiomas")) return "GestionIdiomas";

            // Cierre por defecto
            return "bloqueado_por_defecto";
        }
    }
}
