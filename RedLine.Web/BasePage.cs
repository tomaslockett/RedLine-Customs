using RedLine.Be.Interfaces;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public class BasePage : Page, IObserver
    {
        public string NombrePaginaActual => Path.GetFileName(Request.Path);

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SubjectIdioma.Instancia.AgregarObserver(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            TraducirTodaLaPagina();
        }

        protected override void OnUnload(EventArgs e)
        {
            SubjectIdioma.Instancia.QuitarObserver(this);
            base.OnUnload(e);
        }

        public virtual void ActualizarIdioma(string nuevoIdioma)
        {
            TraducirTodaLaPagina();
        }

        public string Traducir(string clave)
        {
            return SubjectIdioma.Instancia.Traducir(clave);
        }

        public void MostrarAlertaTraducida(string claveMensaje, string prefijo = "")
        {
            string texto = Traducir(claveMensaje);
            if (!string.IsNullOrEmpty(prefijo))
            {
                texto = $"{prefijo} {texto}";
            }

            string script = $"alert('{texto.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), script, true);
        }

        private void TraducirTodaLaPagina()
        {
            // Traduce el título de la pestaña del navegador si existe traducción para la página
            if (!string.IsNullOrEmpty(this.Title))
            {
                string tituloTraducido = SubjectIdioma.Instancia.Traducir(this.Title);
                if (EsTraduccionValida(tituloTraducido))
                {
                    this.Title = tituloTraducido;
                }
            }

            // Traduce la MasterPage (menús, cabeceras, pie de página) si existe
            if (this.Master != null)
            {
                TraducirRecursivo(this.Master);
            }

            // Traduce los controles propios de la página (.aspx)
            TraducirRecursivo(this);
        }

        private void TraducirRecursivo(Control root)
        {
            foreach (Control control in root.Controls)
            {
                if (!string.IsNullOrEmpty(control.ID))
                {
                    string traduccion = SubjectIdioma.Instancia.Traducir(control.ID);

                    if (EsTraduccionValida(traduccion))
                    {
                        switch (control)
                        {
                            // Botones estándar
                            case Button btn:
                                btn.Text = traduccion;
                                break;

                            // Links de navegación y menú
                            case LinkButton lnk:
                                lnk.Text = traduccion;
                                break;

                            // Etiquetas de texto fijas
                            case Label lbl:
                                lbl.Text = traduccion;
                                break;

                            // Literales de texto
                            case Literal lit:
                                lit.Text = traduccion;
                                break;

                            // CheckBoxes y RadioButtons
                            case CheckBox chk:
                                chk.Text = traduccion;
                                break;

                            // TextBoxes: NUNCA se traduce el .Text para no pisar lo que escribe el usuario.
                            // Solo se traduce el placeholder si lo tiene configurado.
                            case TextBox txt when !string.IsNullOrEmpty(txt.Attributes["placeholder"]):
                                txt.Attributes["placeholder"] = traduccion;
                                break;

                            // Grillas: traduce los HeaderText de las columnas
                            case GridView gv:
                                foreach (DataControlField col in gv.Columns)
                                {
                                    if (!string.IsNullOrEmpty(col.HeaderText))
                                    {
                                        string headerTrad = SubjectIdioma.Instancia.Traducir(col.HeaderText);
                                        if (EsTraduccionValida(headerTrad))
                                        {
                                            col.HeaderText = headerTrad;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                if (control.HasControls())
                {
                    TraducirRecursivo(control);
                }
            }
        }

        private bool EsTraduccionValida(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto) && (!texto.StartsWith("[") || !texto.EndsWith("]"));
        }
    }
}