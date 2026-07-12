using Redline.Be;
using RedLine.Bll;
using System;

namespace RedLine.Web
{
    public partial class personalizarAuto : System.Web.UI.Page
    {
        BLL_Auto bllAut = new BLL_Auto();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString["id"] == null)
                    {
                        Response.Redirect("Inventario.aspx");
                        return;
                    }

                    int id = int.Parse(Request.QueryString["id"]);

                    AutoBase auto = bllAut.DevolverAuto(id);

                    if (auto != null)
                    {
                        lblMarca.InnerText = auto.Marca;
                        lblModelo.InnerText = auto.Modelo;
                        lblAnio.InnerText = auto.Anio.ToString();
                        lblPrecio.InnerText = "$ " + auto.PrecioBase.ToString("N2"); 

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
                        throw new Exception("El vehículo solicitado no se encuentra en el stock.");
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message.Replace("'", "\\'") + "'); window.location='Inventario.aspx';</script>");
                }
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Session["Checkout_AutoId"] = Request.QueryString["id"];
            Session["Checkout_Extras"] = Request.Form["extras"];
            Response.Redirect("Checkout.aspx");
        }
    }
}