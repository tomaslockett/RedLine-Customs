using Redline.Be;
using RedLine.Bll;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class GestionClientes : BasePage
    {
        private BLL_Cliente _bllCliente = new BLL_Cliente();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarPanelControl();
            }
        }

        private void CargarPanelControl()
        {
            try
            {
                List<Cliente> listaClientes = _bllCliente.ObtenerClientes();

                dgvClientes.DataSource = listaClientes;
                dgvClientes.DataBind();

                int total = listaClientes.Count;
                lblTotalClientes.Text = total.ToString();
                lblActivosMes.Text = total > 0 ? (total - 1).ToString() : "0";
                lblNuevosMes.Text = total > 0 ? "2" : "0";

                string formatoPaginacion = Traducir("msg_resumen_paginacion_clientes");
                lblResumenPaginacion.Text = string.Format(formatoPaginacion, total);
            }
            catch (Exception)
            {
                MostrarAlertaTraducida("msg_error_cargar_panel_clientes");
            }
        }
        protected void btnImportarXml_Click(object sender, EventArgs e)
        {
            lblMensajeImportacion.Visible = true;

            if (!fuClientesXml.HasFile)
            {
                MostrarMensaje(Traducir("msg_xml_requerido"), false);
                return;
            }

            string extension = Path.GetExtension(fuClientesXml.FileName).ToLower();
            if (extension != ".xml")
            {
                MostrarMensaje(Traducir("msg_xml_extension_invalida"), false);
                return;
            }

            try
            {
                using (var stream = fuClientesXml.PostedFile.InputStream)
                {
                    int importados = _bllCliente.ImportarClientesXML(stream);

                    if (importados > 0)
                    {
                        string formatoExito = Traducir("msg_xml_importacion_exitosa");
                        MostrarMensaje(string.Format(formatoExito, importados), true);
                        CargarPanelControl();
                    }
                    else
                    {
                        MostrarMensaje(Traducir("msg_xml_sin_registros"), false);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"{Traducir("msg_xml_error_procesar")} {ex.Message}", false);
            }
        }
        private void MostrarMensaje(string mensaje, bool esExito)
        {
            lblMensajeImportacion.Text = mensaje;
            lblMensajeImportacion.Style["color"] = esExito ? "#28a745" : "#dc3545"; // Verde para éxito, Rojo para error
            lblMensajeImportacion.Visible = true;
        }
        protected void dgvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerPerfil")
            {
                string clienteDni = e.CommandArgument.ToString();
                Response.Redirect($"PerfilCliente.aspx?dni={clienteDni}");
            }
        }

        protected void dgvClientes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}