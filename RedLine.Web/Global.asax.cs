using RedLine.Servicios;
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


                bool esPaginaPermitida = rutaActual.Contains("recuperaciondv.aspx") ||
                                         rutaActual.Contains("backuprestore.aspx") ||
                                         rutaActual.Contains("login.aspx");

               
                if (context.Session["Inconsistencia"] != null && (bool)context.Session["Inconsistencia"])
                {
                    if (!esPaginaPermitida)
                    {
                        SessionManager.Instancia.Logout();
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                        return; 
                    }
                }

                esPaginaPermitida = rutaActual.Contains("login.aspx");
                if (!SessionManager.Instancia.IsLogged())
                {
                    if (!esPaginaPermitida)
                    {
                        context.Response.Redirect("~/LogIn.aspx", false);
                        context.ApplicationInstance.CompleteRequest();
                    }
                }
            }
        }

    }
}