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
    public partial class CrearAutos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private  BLL_Auto bllAuto = new BLL_Auto();
        protected void Agregar(object sender, EventArgs e)
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(TextBoxID.Text) ||
                    string.IsNullOrWhiteSpace(DropDownListMarca.SelectedValue) ||
                    string.IsNullOrWhiteSpace(TextBoxModelo.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxAño.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxPrecio.Text) ||
                    string.IsNullOrWhiteSpace(TextBoxTipo.Text)
                )
                {
                    throw new Exception("Complete todos los campos obligatorios.");
                }


                if (!FileUploadSubirFoto.HasFile)
                {
                    throw new Exception("Debe seleccionar una imagen.");
                }


                string extension = Path.GetExtension(FileUploadSubirFoto.FileName);

                string nombreArchivo =
                    TextBoxID.Text + extension;
                //Lois
                //Si llegara a haber un error con esto cambiar por:
                // Server.MapPath("~/Content/img/ ") + nombreArchivo;
                string rutaFisica =
                    Server.MapPath("Content/img/") + nombreArchivo;


                FileUploadSubirFoto.SaveAs(rutaFisica);


                string urlImagen =
                    "Content/img/" + nombreArchivo;


                AutoBase auto = new AutoBase
                {
                    CodigoVehiculo = TextBoxID.Text.Trim(),

                    Marca = DropDownListMarca.SelectedValue,

                    Modelo = TextBoxModelo.Text.Trim(),

                    Anio = Convert.ToInt32(TextBoxAño.Text),

                    PrecioBase = Convert.ToDecimal(TextBoxPrecio.Text),

                    Tipo = TextBoxTipo.Text.Trim(),

                    Stock = 1,

                    ImagenUrl = urlImagen,

                    DescripcionGeneral =
                        TextBoxDescripcionGeneral.Text.Trim()
                };



                if (!string.IsNullOrWhiteSpace(TextBoxPotencia.Text))
                {
                    auto.Potencia =
                        Convert.ToInt32(TextBoxPotencia.Text);
                }

                if (!string.IsNullOrWhiteSpace(TextBoxVelocidadMaxima.Text))
                {
                    auto.VelocidadMaxima =
                        Convert.ToInt32(TextBoxVelocidadMaxima.Text);
                }

                if (!string.IsNullOrWhiteSpace(TextBoxAceleracion.Text))
                {
                    auto.Aceleracion0a100 =
                        Convert.ToDecimal(TextBoxAceleracion.Text);
                }


                bllAuto.GuardarAuto(auto);


            }
            catch (Exception ex)
            {
                Response.Write(
                    "<script>alert('" + ex.Message + "');</script>"
                );
            }
        }
        protected void Cancelar(object sender, EventArgs e)
        {

        }
    }
}