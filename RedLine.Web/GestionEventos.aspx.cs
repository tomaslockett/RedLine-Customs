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
    public partial class GestionEventos : System.Web.UI.Page
    {
        private BLL_Evento bllEvento = new BLL_Evento();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ActualizarGrilla();
            }
        }

        protected void Filtro_Changed(object sender, EventArgs e)
        {
            ActualizarGrilla();
        }

        private void ActualizarGrilla()
        {
            List<Evento> lista = bllEvento.ListarTodo();

            if (!string.IsNullOrEmpty(txtFiltroUsuario.Text))
            {
                string filtro = txtFiltroUsuario.Text.Trim().ToLower();
                lista = lista.Where(x => x.Usuario.ToLower().Contains(filtro)).ToList();
            }

            if (!string.IsNullOrEmpty(txtFiltroModulo.Text))
                lista = lista.Where(x => x.Modulo.ToLower().Contains(txtFiltroModulo.Text.ToLower())).ToList();

            if (!string.IsNullOrEmpty(txtFiltroActividad.Text))
                lista = lista.Where(x => x.Actividad.ToLower().Contains(txtFiltroActividad.Text.ToLower())).ToList();

            if (ddlFiltroCri.SelectedValue != "0")
                lista = lista.Where(x => x.Criticidad == int.Parse(ddlFiltroCri.SelectedValue)).ToList();

            if (!string.IsNullOrEmpty(txtFiltroFecha.Text))
            {
                DateTime fec;
                if (DateTime.TryParse(txtFiltroFecha.Text, out fec))
                    lista = lista.Where(x => x.Fecha.Date == fec.Date).ToList();
            }

            gvEventos.DataSource = lista;
            gvEventos.DataBind();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFiltroUsuario.Text = txtFiltroFecha.Text = txtFiltroModulo.Text = txtFiltroActividad.Text = "";
            ddlFiltroCri.SelectedValue = "0";
            ActualizarGrilla();
        }

        public string GetCriColor(object cri)
        {
            int c = Convert.ToInt32(cri);
            if (c >= 3) return "color: #dc3545; font-weight: bold;";
            if (c == 2) return "color: #ffc107;";
            return "color: #28a745;";
        }
        protected void ExportarXML(object sender, EventArgs e)
        {
            try
            {
                BLL_Evento bll = new BLL_Evento();
                byte[] archivoBytes = bll.ExportarBitacoraXmlBytes();

                string nombreArchivo = $"reporteBitacora_{DateTime.Now:dd-MM-yyyy_HHmmss}.xml";

            
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/xml";

                Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");

                Response.BinaryWrite(archivoBytes);
                Response.Flush();
                Response.End(); 
            }
            catch (System.Threading.ThreadAbortException)
            {
              
            }
            catch (UnauthorizedAccessException ex)
            {
                
                MostrarMensajeError("Error de acceso o permisos en el servidor: " + ex.Message);
            }
            catch (Exception ex)
            {
                MostrarMensajeError("Ocurrió un error al exportar la bitácora: " + ex.Message);
            }
        }

        private void MostrarMensajeError(string mensaje)
        {

            ScriptManager.RegisterStartupScript(this, GetType(), "alertError",
                $"alert('{mensaje.Replace("'", "\\'")}');", true);
        }
    }
}