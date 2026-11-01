using RedLine.Be.Entidades;
using RedLine.Be.Interfaces;
using RedLine.Bll;
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
            AsegurarIdiomaCargado();
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

        /// <summary>
        /// Traducción con respaldo: si la clave sigue sin sembrar (llega vacía o "[clave]")
        /// devuelve el texto por defecto pasado como parámetro. Se usa en textos de binding
        /// (Repeater/GridView) que OnPreRender no puede re-traducir ni sanear.
        /// </summary>
        public string TraducirSeguro(string clave, string predeterminado)
        {
            string texto = Traducir(clave);
            if (string.IsNullOrWhiteSpace(texto) || (texto.StartsWith("[") && texto.EndsWith("]")))
            {
                return predeterminado;
            }
            return texto;
        }

        /// <summary>
        /// Garantiza que el diccionario de traducciones de la sesión esté cargado antes
        /// del Page_Load. La MasterPage carga el idioma en su propio Page_Load, que se
        /// ejecuta DESPUÉS del Page_Load del contenido (donde DataBind consume Traducir());
        /// en la primera request el diccionario en sesión está vacío y las etiquetas de
        /// binding renderizan "[clave]". Se respeta el idioma de Session["IdiomaSeleccionado"]
        /// y, si no existe, el idioma default del sistema. Cuando la MasterPage complete su
        /// carga lo reemplazará con el mismo valor, por lo que este respaldo es transparente.
        /// </summary>
        private void AsegurarIdiomaCargado()
        {
            try
            {
                if (SubjectIdioma.Instancia.DiccionarioCargado) return;

                BLL_Idioma bllIdioma = new BLL_Idioma();

                Idioma idioma = null;
                if (Session["IdiomaSeleccionado"] != null)
                {
                    idioma = bllIdioma.ObtenerPorId((int)Session["IdiomaSeleccionado"]);
                }
                if (idioma == null)
                {
                    idioma = bllIdioma.ObtenerDefault() ?? bllIdioma.Listar().FirstOrDefault();
                }

                if (idioma != null)
                {
                    Dictionary<string, string> traducciones = bllIdioma.ObtenerTraduccionesPorIdioma(idioma.ID);
                    SubjectIdioma.Instancia.CargarTraducciones(idioma.Nombre, traducciones);
                }
            }
            catch
            {
                // Sin conexión o sin datos aún: se mantiene el comportamiento previo y
                // la MasterPage reintenta la carga en su Page_Load.
            }
        }

        public void MostrarAlertaTraducida(string claveMensaje, string prefijo = "")
        {
            string texto = Traducir(claveMensaje);
            if (!string.IsNullOrEmpty(prefijo))
            {
                texto = $"{prefijo} {texto}";
            }

            string textoSanitizado = texto.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", "").Replace("\n", "\\n");
            string script = $"alert('{textoSanitizado}');";
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
                    // 1. Grillas: las columnas tienen sus propias claves (ej: col_VentaNumero)
                    // aunque el GridView en sí (ej: dgvVentas) no tenga una etiqueta con su ID.
                    if (control is GridView gv)
                    {
                        TraducirColumnasGridView(gv);
                    }

                    // 2. DropDownLists: sus ítems pueden tener claves propias por índice (ej: ddlFiltroPagina_0)
                    // o por valor (ej: ddlFiltroPrecio_1), además de la etiqueta del control padre.
                    if (control is DropDownList ddl)
                    {
                        TraducirItemsDropDownList(ddl);
                    }

                    // 3. Controles simples cuya clave de traducción es su propio ID
                    string traduccion = SubjectIdioma.Instancia.Traducir(control.ID);

                    if (EsTraduccionValida(traduccion))
                    {
                        switch (control)
                        {
                            // Botones estándar
                            case Button btn:
                                btn.Text = traduccion;
                                break;

                            // Links de navegación con postback
                            case LinkButton lnk:
                                lnk.Text = traduccion;
                                break;

                            // Hipervínculos directos (ej: lnkIrLogin en RegistroCliente.aspx)
                            case HyperLink hlk:
                                hlk.Text = traduccion;
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
                        }
                    }
                }

                if (control.HasControls())
                {
                    TraducirRecursivo(control);
                }
            }
        }

        /// <summary>
        /// Traduce los encabezados de columna de un GridView usando la clave configurada en HeaderText
        /// y actualiza también la fila de cabecera ya renderizada si DataBind() se ejecutó en Page_Load.
        /// </summary>
        private void TraducirColumnasGridView(GridView gv)
        {
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                DataControlField col = gv.Columns[i];
                string viewStateKey = $"HeaderKey_{gv.ID}_{i}";

                // Guardamos la clave original la primera vez para poder retraducir tras un PostBack
                string claveHeader = ViewState[viewStateKey] as string;
                if (string.IsNullOrEmpty(claveHeader) && !string.IsNullOrEmpty(col.HeaderText))
                {
                    claveHeader = col.HeaderText;
                    ViewState[viewStateKey] = claveHeader;
                }

                if (!string.IsNullOrEmpty(claveHeader))
                {
                    string headerTrad = SubjectIdioma.Instancia.Traducir(claveHeader);
                    if (EsTraduccionValida(headerTrad))
                    {
                        col.HeaderText = headerTrad;

                        if (gv.HeaderRow != null && i < gv.HeaderRow.Cells.Count)
                        {
                            gv.HeaderRow.Cells[i].Text = headerTrad;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Traduce el ítem por defecto y cada opción de un DropDownList buscando tanto por índice ({ID}_{i})
        /// como por valor ({ID}_{Value}).
        /// </summary>
        private void TraducirItemsDropDownList(DropDownList ddl)
        {
            string traduccionControl = SubjectIdioma.Instancia.Traducir(ddl.ID);
            if (EsTraduccionValida(traduccionControl) && ddl.Items.Count > 0 && string.IsNullOrEmpty(ddl.Items[0].Value))
            {
                ddl.Items[0].Text = traduccionControl;
            }

            for (int i = 0; i < ddl.Items.Count; i++)
            {
                ListItem item = ddl.Items[i];

                // 1. Buscar por índice (ej: ddlFiltroPagina_0, ddlFiltroCri_0, DropDownListMarca_0)
                string clavePorIndice = $"{ddl.ID}_{i}";
                string tradPorIndice = SubjectIdioma.Instancia.Traducir(clavePorIndice);
                if (EsTraduccionValida(tradPorIndice))
                {
                    item.Text = tradPorIndice;
                    continue;
                }

                // 2. Buscar por Value si no es vacío (ej: ddlFiltroPrecio_1)
                if (!string.IsNullOrEmpty(item.Value))
                {
                    string clavePorValor = $"{ddl.ID}_{item.Value}";
                    string tradPorValor = SubjectIdioma.Instancia.Traducir(clavePorValor);
                    if (EsTraduccionValida(tradPorValor))
                    {
                        item.Text = tradPorValor;
                    }
                }
            }
        }

        private bool EsTraduccionValida(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto) && (!texto.StartsWith("[") || !texto.EndsWith("]"));
        }
    }
}
