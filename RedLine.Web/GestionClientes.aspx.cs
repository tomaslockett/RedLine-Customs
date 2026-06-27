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