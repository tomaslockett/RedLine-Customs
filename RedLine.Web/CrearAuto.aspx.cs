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
    public partial class CrearAuto : System.Web.UI.Page
    {
        private BLL_Auto bllAuto = new BLL_Auto();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Agregar(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextBoxID.Text) || string.IsNullOrWhiteSpace(DropDownListMarca.SelectedValue) || string.IsNullOrWhiteSpace(TextBoxModelo.Text) || string.IsNullOrWhiteSpace(TextBoxAño.Text) || string.IsNullOrWhiteSpace(TextBoxPrecio.Text) || string.IsNullOrWhiteSpace(TextBoxTipo.Text))
                {
                    throw new Exception("Por favor, complete todos los campos obligatorios (*).");
                }

                if (!FileUploadSubirFoto.HasFile)
                {
                    throw new Exception("Debe cargar una imagen representativa para el vehículo.");
                }
                byte[] imagenData = FileUploadSubirFoto.FileBytes;
                AutoBase auto = new AutoBase
                {
                    CodigoVehiculo = TextBoxID.Text.Trim(),
                    Marca = DropDownListMarca.SelectedValue,
                    Modelo = TextBoxModelo.Text.Trim(),
                    Anio = Convert.ToInt32(TextBoxAño.Text),
                    PrecioBase = Convert.ToDecimal(TextBoxPrecio.Text),
                    Tipo = TextBoxTipo.Text.Trim(),
                    Stock = 1,
                    ImagenBinaria = imagenData, 
                    DescripcionGeneral = TextBoxDescripcionGeneral.Text.Trim()
                };

                if (!string.IsNullOrWhiteSpace(TextBoxPotencia.Text))
                {
                    auto.Potencia = Convert.ToInt32(TextBoxPotencia.Text);
                }

                if (!string.IsNullOrWhiteSpace(TextBoxVelocidadMaxima.Text))
                {
                    auto.VelocidadMaxima = Convert.ToInt32(TextBoxVelocidadMaxima.Text);
                }

                if (!string.IsNullOrWhiteSpace(TextBoxAceleracion.Text))
                {
                    auto.Aceleracion0a100 = Convert.ToDecimal(TextBoxAceleracion.Text);
                }

                bllAuto.GuardarAuto(auto);

                Response.Redirect("Inventario.aspx");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        protected void Cancelar(object sender, EventArgs e)
        {
            Response.Redirect("Inventario.aspx");
        }
    }
}
