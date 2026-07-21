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


                bool esPaginaPublica = rutaActual.Contains("login.aspx") || rutaActual.Contains("registrocliente.aspx");

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

        private string ObtenerPermisoRequeridoPorPagina(string rutaActual)
        {
            if (rutaActual.Contains("login.aspx") ||
                  rutaActual.Contains("registrocliente.aspx") ||
                  rutaActual.Contains("logout.aspx"))
                return "libre";


            // Taller y Ventas
            if (rutaActual.Contains("catalogo.aspx")) return "Catalogo";
            if (rutaActual.Contains("crearauto.aspx")) return "GestionInventario";
            if (rutaActual.Contains("personalizarauto.aspx")) return "PersonalizarAuto";
            if (rutaActual.Contains("inventario.aspx")) return "GestionInventario";
            if (rutaActual.Contains("historialventas.aspx")) return "HistorialVentas";

            // Flujo de Compras
            if (rutaActual.Contains("checkout.aspx")) return "Checkout";
            if (rutaActual.Contains("pagoexitoso.aspx")) return "PagoExitoso";

            // Gestión ABM
            if (rutaActual.Contains("gestionusuarios.aspx")) return "GestionUsuarios";
            if (rutaActual.Contains("gestionclientes.aspx")) return "GestionClientes";
            if (rutaActual.Contains("gestioneventos.aspx")) return "BitacoraEventos";

            // Sistema
            if (rutaActual.Contains("gestion_perfiles_permisos.aspx")) return "GestionPerfiles";
            if (rutaActual.Contains("backuprestore.aspx")) return "BackupRestore";
            if (rutaActual.Contains("recuperaciondv.aspx") || rutaActual.Contains("digitoverificador.aspx")) return "RecuperarDV";
            if (rutaActual.Contains("cambiocontraseña.aspx")) return "CambioContraseña";

            // Cierre por defecto
            return "bloqueado_por_defecto";
        }

    }
}