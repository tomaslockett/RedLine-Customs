using Redline.Be;
using RedLine.Bll;
using System;
using System.Globalization;

namespace RedLine.Web
{
    public partial class personalizarAuto : BasePage
    {
        private readonly BLL_Auto _bllAut = new BLL_Auto();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    // Si se accede sin un ID válido (por ejemplo desde el menú superior),
                    // redirigimos a Catalogo.aspx para que tanto clientes como administradores puedan elegir un auto.
                    if (string.IsNullOrEmpty(Request.QueryString["id"]) || !int.TryParse(Request.QueryString["id"], out int id))
                    {
                        Response.Redirect("Catalogo.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    AutoBase auto = _bllAut.DevolverAuto(id);

                    if (auto != null)
                    {
                        lblMarca.InnerText = auto.Marca;
                        lblModelo.InnerText = auto.Modelo;
                        lblAnio.InnerText = auto.Anio.ToString();
                        lblPrecio.InnerText = "$ " + auto.PrecioBase.ToString("N2");
                        lblPrecio.Attributes["data-precio-base"] = auto.PrecioBase.ToString(CultureInfo.InvariantCulture);

                        if (auto.ImagenBinaria != null && auto.ImagenBinaria.Length > 0)
                        {
                            string base64String = Convert.ToBase64String(auto.ImagenBinaria);
                            img.Src = "data:image/jpeg;base64," + base64String;
                        }
                        else
                        {
                            img.Src = "Content/img/auto-reemplazo.png";
                        }
                    }
                    else
                    {
                        throw new Exception(Traducir("msg_auto_no_encontrado_stock"));
                    }
                }
                catch (Exception)
                {
                    Response.Write("<script>alert('" + Traducir("msg_error_general").Replace("'", "\\'") + "'); window.location='Catalogo.aspx';</script>");
                }
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Session["Checkout_AutoId"] = Request.QueryString["id"];
            Session["Checkout_Extras"] = Request.Form["extras"];

            Response.Redirect("Checkout.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
