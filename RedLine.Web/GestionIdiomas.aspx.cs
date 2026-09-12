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
    public partial class GestionIdiomas : BasePage
    {
        private readonly BLL_Idioma _bllIdioma = new BLL_Idioma();
        private readonly BLL_Traduccion _bllTraduccion = new BLL_Traduccion();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarComboIdiomas();
                CargarGrillaTraducciones();
            }
        }

        private void CargarComboIdiomas()
        {
            List<Idioma> idiomas = _bllIdioma.Listar();
            ddlIdiomaDestino.DataSource = idiomas;
            ddlIdiomaDestino.DataTextField = "Nombre";
            ddlIdiomaDestino.DataValueField = "ID";
            ddlIdiomaDestino.DataBind();
        }

        private void CargarGrillaTraducciones()
        {
            if (string.IsNullOrEmpty(ddlIdiomaDestino.SelectedValue)) return;

            int idIdioma = Convert.ToInt32(ddlIdiomaDestino.SelectedValue);
            string paginaFiltro = string.IsNullOrEmpty(ddlFiltroPagina.SelectedValue) ? null : ddlFiltroPagina.SelectedValue;

            List<Traduccion> traducciones = _bllTraduccion.ListarPorIdiomaYPagina(idIdioma, paginaFiltro);

            string filtroTexto = txtFiltro.Text.Trim().ToLower();
            bool soloSinTraducir = chkSoloSinTraducir.Checked;

            var datosFiltrados = traducciones.Where(t =>
                (string.IsNullOrEmpty(filtroTexto) ||
                 t.Etiqueta.Clave.ToLower().Contains(filtroTexto) ||
                 (t.Etiqueta.Descripcion != null && t.Etiqueta.Descripcion.ToLower().Contains(filtroTexto))) &&
                (!soloSinTraducir || string.IsNullOrWhiteSpace(t.Texto))
            ).ToList();

            gvTraducciones.DataSource = datosFiltrados;
            gvTraducciones.DataBind();
        }

        protected void DdlIdiomaDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrillaTraducciones();
        }

        protected void DdlFiltroPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrillaTraducciones();
        }

        protected void TxtFiltro_TextChanged(object sender, EventArgs e)
        {
            CargarGrillaTraducciones();
        }

        protected void ChkSoloSinTraducir_CheckedChanged(object sender, EventArgs e)
        {
            CargarGrillaTraducciones();
        }

        protected void BtnCrearIdioma_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreIdioma.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarMensaje(Traducir("msg_idioma_nombre_vacio"), true);
                return;
            }

            try
            {
                _bllIdioma.GuardarNuevoIdioma(nombre);
                txtNombreIdioma.Text = string.Empty;
                CargarComboIdiomas();

                ListItem itemNuevo = ddlIdiomaDestino.Items.FindByText(nombre);
                if (itemNuevo != null)
                {
                    ddlIdiomaDestino.SelectedValue = itemNuevo.Value;
                }

                CargarGrillaTraducciones();
                MostrarMensaje(Traducir("msg_idioma_creado_exito"), false);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"{Traducir("msg_error_crear_idioma")} {ex.Message}", true);
            }
        }

        protected void BtnGuardarTraducciones_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlIdiomaDestino.SelectedValue)) return;

            int idIdioma = Convert.ToInt32(ddlIdiomaDestino.SelectedValue);
            var traduccionesAGuardar = new List<Traduccion>();

            foreach (GridViewRow fila in gvTraducciones.Rows)
            {
                if (fila.RowType == DataControlRowType.DataRow)
                {
                    int idEtiqueta = Convert.ToInt32(gvTraducciones.DataKeys[fila.RowIndex].Value);
                    TextBox txtTexto = (TextBox)fila.FindControl("txtTextoTraducido");
                    string texto = txtTexto != null ? txtTexto.Text.Trim() : string.Empty;

                    traduccionesAGuardar.Add(new Traduccion
                    {
                        IdIdioma = idIdioma,
                        IdEtiqueta = idEtiqueta,
                        Texto = texto
                    });
                }
            }

            try
            {
                _bllTraduccion.GuardarTraducciones(idIdioma, traduccionesAGuardar);

                if (SubjectIdioma.Instancia.IdiomaActual == ddlIdiomaDestino.SelectedItem.Text)
                {
                    Dictionary<string, string> diccionarioActualizado = _bllIdioma.ObtenerTraduccionesPorIdioma(idIdioma);
                    SubjectIdioma.Instancia.CargarTraducciones(ddlIdiomaDestino.SelectedItem.Text, diccionarioActualizado);
                }

                CargarGrillaTraducciones();
                MostrarMensaje(Traducir("msg_traducciones_guardadas"), false);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"{Traducir("msg_error_guardar_traduccion")} {ex.Message}", true);
            }
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblEstado.Text = texto;
            lblEstado.ForeColor = esError ? System.Drawing.Color.FromName("#D93416") : System.Drawing.Color.Green;
        }
    }
}