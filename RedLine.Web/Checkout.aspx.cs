using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.BLL;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class Checkout : BasePage
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
                    string extrasHtml = string.Empty;
                    string extrasSeleccionados = Session["Checkout_Extras"]?.ToString() ?? string.Empty;

                    var preciosExtras = new Dictionary<string, decimal>
                    {
                        { "aleron", 500 },
                        { "kitCarroceria", 1200 },
                        { "llantas", 2500 },
                        { "suspension", 3000 },
                        { "pintura", 1800 }
                    };

                    var clavesExtras = new Dictionary<string, string>
                    {
                        { "aleron", "extra_aleron" },
                        { "kitCarroceria", "extra_kitCarroceria" },
                        { "llantas", "extra_llantas" },
                        { "suspension", "extra_suspension" },
                        { "pintura", "extra_pintura" }
                    };

                    if (!string.IsNullOrEmpty(extrasSeleccionados))
                    {
                        string[] extrasArray = extrasSeleccionados.Split(',');
                        foreach (string extra in extrasArray)
                        {
                            if (preciosExtras.ContainsKey(extra))
                            {
                                subtotal += preciosExtras[extra];
                                string nombreTraducido = Traducir(clavesExtras[extra]);

                                extrasHtml += $@"
                                    <div class='price-row'>
                                        <span>{nombreTraducido}</span>
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
            try
            {

                if (Session["Checkout_AutoId"] == null)
                {
                    Response.Redirect("Inventario.aspx");
                    return;
                }

                int idAutoBase = int.Parse(Session["Checkout_AutoId"].ToString());
                string extrasSeleccionados = Session["Checkout_Extras"]?.ToString() ?? string.Empty;


                if (!SessionManager.Instancia.IsLogged())
                {
                    Response.Redirect("Login.aspx");
                    return;
                }




                var usuarioLogueado = new BLL_Cliente().BuscarPorEmail(SessionManager.Instancia.Usuario.Email);

                if (usuarioLogueado == null)
                {
                    throw new Exception("No se pudo recuperar la información del usuario logueado.");
                }


                BLL_Auto bllAuto = new BLL_Auto();
                AutoBase autoBase = bllAuto.DevolverAuto(idAutoBase);

                decimal subtotal = autoBase.PrecioBase;



                AutoPersonalizado autoPersonalizado = null;
                var listaMejoras = new List<Mejora>();


                if (!string.IsNullOrEmpty(extrasSeleccionados))
                {
                    string[] extrasArray = extrasSeleccionados.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    BLL_Mejora bllMejora = new BLL_Mejora();

                    foreach (string extra in extrasArray)
                    {

                        Mejora mejoraBd = bllMejora.ObtenerPorClaveHtml(extra.Trim());

                        if (mejoraBd != null)
                        {
                            subtotal += mejoraBd.Precio;
                            listaMejoras.Add(mejoraBd); 
                        }
                    }

                    autoPersonalizado = new AutoPersonalizado
                    {
                        AuBase = autoBase,
                        Cliente = usuarioLogueado,
                        DNI_Cliente = usuarioLogueado.DNI,
                        Mejoras = listaMejoras
                    };

                    bllAuto.GuardarAutoEnGarage(autoPersonalizado);
                }

                decimal ivaCalc = subtotal * 0.21m;
                decimal totalFinal = subtotal + ivaCalc;


                Venta nuevaVenta = new Venta
                {
                    NumeroVenta = "V-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Fecha = DateTime.Now,
                    IVA = 21,
                    Total = totalFinal,
                    Cliente = usuarioLogueado,
                    AutoBase = autoBase,
                    AutoPersonalizado = autoPersonalizado
                };


                Factura nuevaFactura = new Factura
                {
                    NumeroFactura = "F-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    FechaEmision = DateTime.Now,
                    MetodoPago = "Tarjeta de Crédito",
                    IVA = 21,
                    Total = totalFinal
                };


                BLL_Venta bllVenta = new BLL_Venta();
                bllVenta.CrearVenta(nuevaVenta, nuevaFactura);


                Session["Checkout_AutoId"] = null;
                Session["Checkout_Extras"] = null;

                Response.Redirect("PagoExitoso.aspx");
            }
            catch (Exception ex)
            {

            }
        }


    }
}