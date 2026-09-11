using RedLine.Be.Entidades;
using RedLine.Be.Interfaces;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class GestionIdiomas : Page, IObserver
    {
        private BLL_Idioma _bllIdioma = new BLL_Idioma();

        protected void Page_Load(object sender, EventArgs e)
        {
            SubjectIdioma.Instancia.AgregarObserver(this);

            if (!IsPostBack)
            {
                ActualizarIdioma(SubjectIdioma.Instancia.IdiomaActual);
                CargarComboIdiomas();
                CargarGrillaTraducciones();
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            SubjectIdioma.Instancia.QuitarObserver(this);
        }

        public void ActualizarIdioma(string nuevoIdioma)
        {
            lblTituloPagina.Text = SubjectIdioma.Instancia.Traducir("lblTituloGestionIdiomas");
            lblSubtituloNuevoIdioma.Text = SubjectIdioma.Instancia.Traducir("lblSubtituloNuevoIdioma");
            btnCrearIdioma.Text = SubjectIdioma.Instancia.Traducir("btnCrearIdioma");
            lblSubtituloTraducciones.Text = SubjectIdioma.Instancia.Traducir("lblSubtituloTraducciones");
            lblSeleccionarIdioma.Text = SubjectIdioma.Instancia.Traducir("lblSeleccionarIdioma");
            lblBuscar.Text = SubjectIdioma.Instancia.Traducir("lblBuscarEtiqueta");
            btnGuardarTraducciones.Text = SubjectIdioma.Instancia.Traducir("btnGuardarTraducciones");
        }

        private void CargarComboIdiomas()
        {
            List<Idioma> idiomas = _bllIdioma.Listar();
            ddlIdiomaDestino.DataSource = idiomas;
            ddlIdiomaDestino.DataTextField = "Nombre";
            ddlIdiomaDestino.DataValueField = "Id";
            ddlIdiomaDestino.DataBind();
        }

        private void CargarGrillaTraducciones()
        {
            if (ddlIdiomaDestino.SelectedValue == null || string.IsNullOrEmpty(ddlIdiomaDestino.SelectedValue))
                return;

            int idIdioma = Convert.ToInt32(ddlIdiomaDestino.SelectedValue);
            Dictionary<string, string> traducciones = _bllIdioma.ObtenerTraduccionesPorIdioma(idIdioma);

            string filtro = txtFiltro.Text.Trim().ToLower();

            var datosGrilla = traducciones
                .Where(t => string.IsNullOrEmpty(filtro) || t.Key.ToLower().Contains(filtro))
                .Select(t => new { EtiquetaKey = t.Key, Texto = t.Value })
                .ToList();

            gvTraducciones.DataSource = datosGrilla;
            gvTraducciones.DataBind();
        }

        protected void DdlIdiomaDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrillaTraducciones();
        }

        protected void TxtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarGrillaTraducciones();
        }

        protected void BtnCrearIdioma_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreIdioma.Text.Trim();

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarMensaje("Ingrese un nombre válido para el idioma.", true);
                return;
            }

            try
            {
                _bllIdioma.GuardarNuevoIdioma(nombre);
                txtNombreIdioma.Text = string.Empty;
                CargarComboIdiomas();
                CargarGrillaTraducciones();
                MostrarMensaje("Idioma creado exitosamente.", false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al crear el idioma: " + ex.Message, true);
            }
        }

        protected void BtnGuardarTraducciones_Click(object sender, EventArgs e)
        {
            if (ddlIdiomaDestino.SelectedValue == null) return;

            int idIdioma = Convert.ToInt32(ddlIdiomaDestino.SelectedValue);
            Dictionary<string, string> traduccionesNuevas = new Dictionary<string, string>();

            foreach (GridViewRow fila in gvTraducciones.Rows)
            {
                if (fila.RowType == DataControlRowType.DataRow)
                {
                    string key = gvTraducciones.DataKeys[fila.RowIndex].Value.ToString();
                    TextBox txtTexto = (TextBox)fila.FindControl("txtTextoTraducido");
                    string texto = txtTexto != null ? txtTexto.Text : string.Empty;

                    traduccionesNuevas[key] = texto;
                }
            }

            try
            {
                _bllIdioma.GuardarTraducciones(idIdioma, traduccionesNuevas);

                if (Session["IdiomaSeleccionado"] != null && (int)Session["IdiomaSeleccionado"] == idIdioma)
                {
                    SubjectIdioma.Instancia.CargarTraducciones(ddlIdiomaDestino.SelectedItem.Text, traduccionesNuevas);
                }

                MostrarMensaje("Traducciones guardadas correctamente.", false);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar traducciones: " + ex.Message, true);
            }
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblEstado.Text = texto;
            lblEstado.ForeColor = esError ? System.Drawing.Color.FromName("#D93416") : System.Drawing.Color.Green;
        }
    }
}