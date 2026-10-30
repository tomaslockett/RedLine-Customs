using Redline.Be;
using RedLine.Bll;
using System;
using System.Linq;

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
                    int id = 0;
                    if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int parsedId))
                    {
                        id = parsedId;
                    }
                    else if (Session["Checkout_AutoId"] != null && int.TryParse(Session["Checkout_AutoId"].ToString(), out int sesId))
                    {
                        id = sesId;
                    }
                    else
                    {
                        var autosDisponibles = _bllAut.MostrarAutosBase();
                        if (autosDisponibles != null && autosDisponibles.Count > 0)
                        {
                            id = autosDisponibles.First().ID;
                        }
                        else
                        {
                            Response.Redirect("Catalogo.aspx");
                            return;
                        }
                    }

                    AutoBase auto = _bllAut.DevolverAuto(id);

                    if (auto != null)
                    {
                        hfAutoId.Value = auto.ID.ToString();
                        lblMarca.InnerText = auto.Marca;
                        lblModelo.InnerText = auto.Modelo;
                        lblAnio.InnerText = auto.Anio.ToString();
                        lblPrecio.InnerText = "$ " + auto.PrecioBase.ToString("N2");
                        lblPrecioBaseHidden.InnerText = auto.PrecioBase.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        if (auto.ImagenBinaria != null && auto.ImagenBinaria.Length > 0)
                        {
                            string base64String = Convert.ToBase64String(auto.ImagenBinaria);
                            img.Src = "data:image/jpeg;base64," + base64String;
                        }
                        else
                        {
                            img.Src = "Content/img/porsche.jfif";
                        }
                    }
                    else
                    {
                        throw new Exception(Traducir("msg_auto_no_encontrado_stock"));
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message.Replace("'", "\\'") + "'); window.location='Catalogo.aspx';</script>");
                }
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            string idAuto = !string.IsNullOrEmpty(hfAutoId.Value) ? hfAutoId.Value : Request.QueryString["id"];
            Session["Checkout_AutoId"] = idAuto;
            Session["Checkout_Extras"] = Request.Form["extras"];
            Response.Redirect("Checkout.aspx");
        }
    }
}
