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
    public partial class Checkout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosCheckout();
            }
        }

        private void CargarDatosCheckout()
        {
            if (Session["Checkout_AutoId"] != null)
            {
                int idAuto = int.Parse(Session["Checkout_AutoId"].ToString());
                BLL_Auto bllAut = new BLL_Auto();
                AutoBase auto = bllAut.DevolverAuto(idAuto);

                if (auto != null)
                {
                    lblModelo.InnerText = $"{auto.Marca} {auto.Modelo}";
                    lblPrecioBase.InnerText = $"${auto.PrecioBase:N0}";

                    if (auto.ImagenBinaria != null && auto.ImagenBinaria.Length > 0)
                    {
                        string base64String = Convert.ToBase64String(auto.ImagenBinaria);
                        imgAuto.Src = "data:image/jpeg;base64," + base64String;
                    }
                    else
                    {
                        imgAuto.Src = "Content/img/auto-reemplazo.png";
                    }

                    decimal subtotal = auto.PrecioBase;
                    string extrasHtml = "";
                    string extrasSeleccionados = Session["Checkout_Extras"]?.ToString() ?? "";

                    var preciosExtras = new Dictionary<string, decimal>
                    {
                        { "aleron", 500 },
                        { "kitCarroceria", 1200 },
                        { "llantas", 2500 },
                        { "suspension", 3000 },
                        { "pintura", 1800 }
                    };
                    var nombresExtras = new Dictionary<string, string>
                    {
                        { "aleron", "Alerón deportivo" },
                        { "kitCarroceria", "Kit de carrocería" },
                        { "llantas", "Llantas personalizadas" },
                        { "suspension", "Suspensión ajustable" },
                        { "pintura", "Pintura personalizada" }
                    };

                    if (!string.IsNullOrEmpty(extrasSeleccionados))
                    {
                        string[] extrasArray = extrasSeleccionados.Split(',');
                        foreach (string extra in extrasArray)
                        {
                            if (preciosExtras.ContainsKey(extra))
                            {
                                subtotal += preciosExtras[extra];
                                extrasHtml += $@"
                                    <div class='price-row'>
                                        <span>{nombresExtras[extra]}</span>
                                        <span>+${preciosExtras[extra]:N0}</span>
                                    </div>";
                            }
                        }
                    }

                    contenedorExtras.InnerHtml = extrasHtml;

                    decimal iva = subtotal * 0.21m;
                    decimal total = subtotal + iva;

                    lblSubtotal.InnerText = $"${subtotal:N0}";
                    lblIva.InnerText = $"${iva:N0}";
                    lblTotal.InnerText = $"${total:N0}";
                }
            }
            else
            {
                Response.Redirect("Inventario.aspx");
            }
        }

        protected void btnProcesarPago_Click(object sender, EventArgs e)
        {
            Session["Checkout_AutoId"] = null;
            Session["Checkout_Extras"] = null;
            Response.Redirect("PagoExitoso.aspx");
        }
    }
}