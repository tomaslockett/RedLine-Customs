using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
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
                InyectarTraduccionesClienteJs();
            }
        }

        private void InyectarTraduccionesClienteJs()
        {
            var textosJs = new Dictionary<string, string>
            {
                { "tarjetaValida", TraducirSeguro("js_tarjeta_valida", "Tarjeta válida") },
                { "tarjetaInvalida", TraducirSeguro("js_tarjeta_invalida", "Número de tarjeta inválido") },
                { "calculando", TraducirSeguro("js_calculando", "Calculando cotización...") },
                { "errorServicio", TraducirSeguro("js_error_servicio", "Servicio de financiación no disponible momentáneamente") },
                { "cuotasDe", TraducirSeguro("js_cuotas_de", "cuotas fijas de") }
            };

            string json = new JavaScriptSerializer().Serialize(textosJs);
            string script = $"window.CheckoutI18n = {json};";
            ClientScript.RegisterStartupScript(this.GetType(), "CheckoutI18nScript", script, true);
        }

        private void CargarDatosCheckout()
        {
            if (Session["Checkout_AutoId"] != null && int.TryParse(Session["Checkout_AutoId"].ToString(), out int idAuto))
            {
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
                        imgAuto.Src = "Content/img/porsche.jfif";
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
                        string[] extrasArray = extrasSeleccionados.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string extraRaw in extrasArray)
                        {
                            string extra = extraRaw.Trim();
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

                    decimal iva = Math.Round(subtotal * 0.21m, 2);
                    decimal total = subtotal + iva;

                    lblSubtotal.InnerText = $"${subtotal:N0}";
                    lblIva.InnerText = $"${iva:N0}";
                    lblTotal.InnerText = $"${total:N0}";

                    if (hfTotalUsd != null)
                    {
                        hfTotalUsd.Value = total.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    Response.Redirect("Catalogo.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                Response.Redirect("Catalogo.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnProcesarPago_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblErrorCheckout != null)
                {
                    lblErrorCheckout.Visible = false;
                    lblErrorCheckout.Text = string.Empty;
                }

                if (Session["Checkout_AutoId"] == null || !int.TryParse(Session["Checkout_AutoId"].ToString(), out int idAutoBase))
                {
                    Response.Redirect("Catalogo.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Verificamos que haya una sesión activa (sea Usuario o ClienteSession)
                bool hayUsuario = SessionManager.Instancia.IsLogged();
                bool hayCliente = Session["ClienteSession"] != null;

                if (!hayUsuario && !hayCliente)
                {
                    Response.Redirect("LogIn.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Determinamos el método de pago elegido (contrato de datos persistido)
                string metodoPago = TextosComercialesVentas.MetodoPagoTarjeta;
                if (hfMetodoPago != null && !string.IsNullOrEmpty(hfMetodoPago.Value))
                {
                    metodoPago = hfMetodoPago.Value;
                }

                // Si paga con tarjeta, validamos campos básicos y algoritmo de Luhn mediante FinanciacionService
                if (metodoPago == TextosComercialesVentas.MetodoPagoTarjeta)
                {
                    string titular = txtCardName != null ? txtCardName.Text.Trim() : string.Empty;
                    string numeroTarjeta = txtCardNumber != null ? txtCardNumber.Text.Replace(" ", "").Trim() : string.Empty;
                    string vencimiento = txtExpiry != null ? txtExpiry.Text.Trim() : string.Empty;
                    string cvv = txtCvv != null ? txtCvv.Text.Trim() : string.Empty;

                    if (string.IsNullOrEmpty(titular) || numeroTarjeta.Length < 12 || vencimiento.Length < 4 || cvv.Length < 3)
                    {
                        if (lblErrorCheckout != null)
                        {
                            lblErrorCheckout.Text = TraducirSeguro("msg_error_datos_tarjeta_checkout", "Por favor completá todos los datos de la tarjeta antes de confirmar el pago.");
                            lblErrorCheckout.Visible = true;
                        }
                        return;
                    }

                    // Validación de servidor por algoritmo de Luhn (100% offline)
                    FinanciacionService service = new FinanciacionService();
                    ResultadoValidacionTarjetaDto validacion = service.ValidarTarjeta(numeroTarjeta);
                    if (!validacion.EsValida)
                    {
                        if (lblErrorCheckout != null)
                        {
                            lblErrorCheckout.Text = TraducirSeguro("msg_error_tarjeta_invalida_luhn", "El número de tarjeta ingresado no es válido según el algoritmo de verificación.");
                            lblErrorCheckout.Visible = true;
                        }
                        return;
                    }
                }

                Cliente clienteComprador = ObtenerOCrearClienteActual();
                if (clienteComprador == null)
                {
                    throw new Exception("No se pudo recuperar la información del cliente logueado.");
                }

                BLL_Auto bllAuto = new BLL_Auto();
                AutoBase autoBase = bllAuto.DevolverAuto(idAutoBase);
                if (autoBase == null)
                {
                    throw new Exception("El vehículo seleccionado ya no se encuentra disponible.");
                }

                decimal subtotal = autoBase.PrecioBase;
                string extrasSeleccionados = Session["Checkout_Extras"]?.ToString() ?? string.Empty;
                List<Mejora> listaMejoras = new List<Mejora>();

                if (!string.IsNullOrEmpty(extrasSeleccionados))
                {
                    string[] extrasArray = extrasSeleccionados.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    BLL_Mejora bllMejora = new BLL_Mejora();

                    foreach (string extraRaw in extrasArray)
                    {
                        string claveExtra = extraRaw.Trim();
                        Mejora mejoraElegida = null;

                        try
                        {
                            mejoraElegida = bllMejora.ObtenerPorClaveHtml(claveExtra);
                        }
                        catch
                        {
                            mejoraElegida = null;
                        }

                        // Respaldo por si la tabla Mejora aún no fue poblada por el Seed en la base de datos
                        if (mejoraElegida == null)
                        {
                            mejoraElegida = CrearMejoraRespaldoPorClave(claveExtra);
                        }

                        if (mejoraElegida != null)
                        {
                            subtotal += mejoraElegida.Precio;
                            listaMejoras.Add(mejoraElegida);
                        }
                    }
                }

                decimal ivaCalc = Math.Round(subtotal * 0.21m, 2);
                decimal totalFinal = subtotal + ivaCalc;

                string marcaTiempo = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string numVenta = "VTA-" + marcaTiempo;
                string numFactura = "FC-B-0001-" + DateTime.Now.ToString("HHmmssff");

                // Armamos la descripción de las mejoras para mostrar en MiGaraje, Facturación e HistorialVentas
                // Centinela persistido en BD (ver TextosComercialesVentas); la UI lo traduce al mostrarlo.
                string textoMejoras = TextosComercialesVentas.ConfiguracionEstandar;
                if (listaMejoras.Count > 0)
                {
                    List<string> nombresMejoras = new List<string>();
                    foreach (Mejora m in listaMejoras)
                    {
                        nombresMejoras.Add(m.Nombre);
                    }
                    textoMejoras = string.Join(", ", nombresMejoras);
                }

                // Siempre creamos el AutoPersonalizado (incluso sin extras) para que figure en MiGaraje.aspx
                AutoPersonalizado autoPersonalizado = new AutoPersonalizado
                {
                    AuBase = autoBase,
                    Cliente = clienteComprador,
                    DNI_Cliente = clienteComprador.DNI,
                    Mejoras = listaMejoras,
                    NombreExtra = textoMejoras,
                    FechaCompra = DateTime.Now,
                    NumeroVenta = numVenta,
                    NumeroFactura = numFactura,
                    MetodoPago = metodoPago,
                    TotalPagado = totalFinal
                };

                try
                {
                    bllAuto.GuardarAutoEnGarage(autoPersonalizado);
                }
                catch
                {
                    // Si la base de datos no tiene cargadas aún las mejoras con stock o ID, asignamos un ID en memoria
                    if (autoPersonalizado.ID <= 0)
                    {
                        autoPersonalizado.ID = new Random().Next(1000, 99999);
                    }
                }

                Venta nuevaVenta = new Venta
                {
                    NumeroVenta = numVenta,
                    Fecha = DateTime.Now,
                    IVA = 21,
                    Total = totalFinal,
                    MetodoPago = metodoPago,
                    Cliente = clienteComprador,
                    AutoBase = autoBase,
                    AutoPersonalizado = autoPersonalizado
                };

                Factura nuevaFactura = new Factura
                {
                    NumeroFactura = numFactura,
                    FechaEmision = DateTime.Now,
                    MetodoPago = metodoPago,
                    IVA = 21,
                    Total = totalFinal,
                    Venta = nuevaVenta
                };

                nuevaVenta.Factura = nuevaFactura;

                BLL_Venta bllVenta = new BLL_Venta();
                bllVenta.CrearVenta(nuevaVenta, nuevaFactura);

                // Guardamos los comprobantes en sesión para PagoExitoso, MiGaraje y Facturación
                Session["UltimaTransaccion"] = nuevaVenta.NumeroVenta;
                Session["UltimaVenta"] = nuevaVenta;
                Session["UltimaFactura"] = nuevaFactura;
                Session["ClienteSession"] = clienteComprador;

                Session["Checkout_AutoId"] = null;
                Session["Checkout_Extras"] = null;

                Response.Redirect("PagoExitoso.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                if (lblErrorCheckout != null)
                {
                    lblErrorCheckout.Text = TraducirSeguro("msg_error_procesar_compra", "Ocurrió un error al procesar la compra. Intente nuevamente.");
                    lblErrorCheckout.Visible = true;
                }
            }
        }

        /// <summary>
        /// Obtiene el Cliente actualmente autenticado (desde Session["ClienteSession"] o desde SessionManager).
        /// Si el usuario logueado del sistema aún no tiene registro en la tabla Cliente, lo crea para poder facturar.
        /// </summary>
        private Cliente ObtenerOCrearClienteActual()
        {
            BLL_Cliente bllCliente = new BLL_Cliente();

            if (Session["ClienteSession"] is Cliente clienteSesion)
            {
                if (!string.IsNullOrEmpty(clienteSesion.Email))
                {
                    Cliente desdeBd = bllCliente.BuscarPorEmail(clienteSesion.Email);
                    if (desdeBd != null)
                    {
                        Session["ClienteSession"] = desdeBd;
                        return desdeBd;
                    }
                }
                return clienteSesion;
            }

            if (SessionManager.Instancia.IsLogged() && SessionManager.Instancia.Usuario != null)
            {
                Usuario u = SessionManager.Instancia.Usuario;
                Cliente clienteExistente = bllCliente.BuscarPorEmail(u.Email);
                if (clienteExistente != null)
                {
                    Session["ClienteSession"] = clienteExistente;
                    return clienteExistente;
                }

                Cliente nuevoCliente = new Cliente
                {
                    DNI = !string.IsNullOrEmpty(u.DNI) ? u.DNI : "11111111",
                    Nombre = !string.IsNullOrEmpty(u.Nombre) ? u.Nombre : "Cliente",
                    Apellido = !string.IsNullOrEmpty(u.Apellido) ? u.Apellido : "RedLine",
                    Email = u.Email,
                    Contraseña = u.Contraseña,
                    Telefono = "1140003000",
                    Direccion = "Buenos Aires, Argentina"
                };

                try
                {
                    bllCliente.Insertar(nuevoCliente);
                    Cliente insertado = bllCliente.BuscarPorEmail(u.Email);
                    if (insertado != null)
                    {
                        nuevoCliente = insertado;
                    }
                }
                catch
                {
                }

                Session["ClienteSession"] = nuevoCliente;
                return nuevoCliente;
            }

            return null;
        }

        /// <summary>
        /// Crea un objeto Mejora con sus valores estándar cuando la tabla Mejora de SQL está vacía.
        /// </summary>
        private Mejora CrearMejoraRespaldoPorClave(string claveHtml)
        {
            switch (claveHtml)
            {
                case "aleron":
                    return new Mejora { ID = 1, Nombre = "Alerón Deportivo", Precio = 500m, Stock = 10, Categoria = "Aerodinámica" };
                case "kitCarroceria":
                    return new Mejora { ID = 2, Nombre = "Kit de Carrocería", Precio = 1200m, Stock = 10, Categoria = "Estética" };
                case "llantas":
                    return new Mejora { ID = 3, Nombre = "Llantas de Aleación", Precio = 2500m, Stock = 10, Categoria = "Ruedas" };
                case "suspension":
                    return new Mejora { ID = 4, Nombre = "Suspensión Deportiva", Precio = 3000m, Stock = 10, Categoria = "Performance" };
                case "pintura":
                    return new Mejora { ID = 5, Nombre = "Pintura Especial", Precio = 1800m, Stock = 10, Categoria = "Estética" };
                default:
                    return null;
            }
        }
    }
}
