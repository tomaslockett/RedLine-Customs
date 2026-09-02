using Redline.Be;
using RedLine.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class GestionClientes : System.Web.UI.Page
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

                lblResumenPaginacion.Text = $"Mostrando 1–{total} de {total} clientes registrados";
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error al cargar el panel de administración de clientes.');</script>");
            }
        }
        protected void btnImportarXml_Click(object sender, EventArgs e)
        {
            lblMensajeImportacion.Visible = true;


            if (!fuClientesXml.HasFile)
            {
                MostrarMensaje("Por favor, seleccione un archivo XML.", false);
                return;
            }

            string extension = System.IO.Path.GetExtension(fuClientesXml.FileName).ToLower();
            if (extension != ".xml")
            {
                MostrarMensaje("El archivo seleccionado no es un XML válido.", false);
                return;
            }

            try
            {

                using (var stream = fuClientesXml.PostedFile.InputStream)
                {
                    int importados = _bllCliente.ImportarClientesXML(stream);

                    if (importados > 0)
                    {
                        MostrarMensaje($"¡Proceso completado! Se importaron {importados} clientes correctamente.", true);
                        CargarPanelControl(); 
                    }
                    else
                    {
                        MostrarMensaje("No se importó ningún cliente. Todos los registros eran duplicados o el archivo no tenía contenido válido.", false);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al procesar el archivo: {ex.Message}", false);
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