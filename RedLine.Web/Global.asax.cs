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
                string rutaActual = context.Request.AppRelativeCurrentExecutionFilePath.ToLower();


                bool esPermitidaEnInconsistencia = rutaActual.Contains("recuperaciondv.aspx") ||
                                                   rutaActual.Contains("backuprestore.aspx") ||
                                                   rutaActual.Contains("login.aspx");

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

                        if (SessionManager.Instancia.IsLogged())
                        {
                            int idPerfilUsuario = (int)SessionManager.Instancia.Usuario.PerfilId;
                            var gestorPerfil = new BLL_Perfil();
                            List<ComponentePermiso> permisosUsuario = gestorPerfil.ObtenerPermisosDePerfil(idPerfilUsuario);


                            tienePermisoSeguridad = permisosUsuario.Any(p => p.Nombre.ToLower() == "gestionseguridad");
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


                bool esPaginaPublica = rutaActual.Contains("login.aspx") ||
                                       rutaActual.Contains("registrocliente.aspx");

                if (!SessionManager.Instancia.IsLogged())
                {
                    if (!esPaginaPublica)
                    {
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                    }
                    return;
                }


                if (SessionManager.Instancia.IsLogged())
                {
                    string permisoRequerido = ObtenerPermisoRequeridoPorPagina(rutaActual);

                    if (permisoRequerido != "libre")
                    {
                        int idPerfilUsuario = (int)SessionManager.Instancia.Usuario.PerfilId;
                        var gestorPerfil = new BLL_Perfil();
                        List<ComponentePermiso> permisosUsuario = gestorPerfil.ObtenerPermisosDePerfil(idPerfilUsuario);

                        bool tienePermiso = permisosUsuario.Any(p => p.Nombre.ToLower() == permisoRequerido.ToLower());

                        if (!tienePermiso)
                        {
                            context.Response.Redirect("~/Catalogo.aspx", false);
                            context.ApplicationInstance.CompleteRequest();
                        }
                    }
                }
            }
        }

        private string ObtenerPermisoRequeridoPorPagina(string ruta)
        {
            if (ruta.Contains("logout.aspx") || ruta.Contains("cambiocontraseña.aspx") || ruta.Contains("login.aspx") || ruta.Contains("registrocliente.aspx"))
                return "libre";

            if (ruta.Contains("catalogo.aspx") || ruta.Contains("personalizarauto.aspx"))
                return "Catalogo";

            if (ruta.Contains("checkout.aspx") || ruta.Contains("pagoexitoso.aspx"))
                return "RealizarCompra";

            if (ruta.Contains("inventario.aspx") || ruta.Contains("historialventas.aspx") || ruta.Contains("crearauto.aspx"))
                return "GestionInventario";

            if (ruta.Contains("gestionclientes.aspx"))
                return "GestionClientes";

            if (ruta.Contains("gestioneventos.aspx"))
                return "BitacoraEventos";

            if (ruta.Contains("gestionusuarios.aspx"))
                return "GestionUsuarios";

            if (ruta.Contains("gestion_perfiles_permisos.aspx") || ruta.Contains("backuprestore.aspx") || ruta.Contains("recuperaciondv.aspx"))
                return "GestionSeguridad";

            return "bloqueado_por_defecto";
        }

    }
}