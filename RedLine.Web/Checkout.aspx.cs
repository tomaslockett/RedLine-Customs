using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.BLL;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace RedLine.Web
{
    public partial class Checkout : BasePage
    {
        private readonly BLL_Auto _bllAuto = new BLL_Auto();
        private readonly BLL_Venta _bllVenta = new BLL_Venta();
        private readonly BLL_Cliente _bllCliente = new BLL_Cliente();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosCheckout();
                AutoCompletarDatosCliente();
            }
        }

        private void CargarDatosCheckout()
        {
            int idAuto = 0;
            if (Session["Checkout_AutoId"] != null && int.TryParse(Session["Checkout_AutoId"].ToString(), out int parsedId))
            {
                idAuto = parsedId;
            }
            else
            {
                var autos = _bllAuto.MostrarAutosBase();
                if (autos != null && autos.Count > 0)
                {
                    idAuto = autos.First().ID;
                    Session["Checkout_AutoId"] = idAuto.ToString();
                }
                else
                {
                    Response.Redirect("Catalogo.aspx");
                    return;
                }
            }

            AutoBase auto = _bllAuto.DevolverAuto(idAuto);

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

                var preciosExtras = new Dictionary<string, (string Titulo, decimal Precio)>
                {
                    { "aleron", ("Alerón deportivo", 500m) },
                    { "kitCarroceria", ("Kit de carrocería", 1200m) },
                    { "llantas", ("Llantas personalizadas", 2500m) },
                    { "suspension", ("Suspensión ajustable", 3000m) },
                    { "pintura", ("Pintura personalizada", 1800m) }
                };

                if (!string.IsNullOrEmpty(extrasSeleccionados))
                {
                    string[] extrasArray = extrasSeleccionados.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string extra in extrasArray)
                    {
                        string limpia = extra.Trim();
                        if (preciosExtras.ContainsKey(limpia))
                        {
                            var info = preciosExtras[limpia];
                            subtotal += info.Precio;

                            extrasHtml += $@"
                                <div class='price-row'>
                                    <span><i class='fa-solid fa-check' style='color:#cf2a1d; margin-right:6px;'></i>{info.Titulo}</span>
                                    <span>+${info.Precio:N0}</span>
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
            }
            else
            {
                Response.Redirect("Catalogo.aspx");
            }
        }

        private void AutoCompletarDatosCliente()
        {
            try
            {
                Cliente cliente = ObtenerClienteActual();
                if (cliente != null)
                {
                    string nombreCompleto = $"{cliente.Nombre} {cliente.Apellido}".Trim();
                    txtCardName.Text = nombreCompleto;
                    txtCardDni.Text = cliente.DNI;

                    txtTransferTitular.Text = nombreCompleto;
                    txtTransferDni.Text = cliente.DNI;
                    txtTransferTelefono.Text = cliente.Telefono;
                }
            }
            catch
            {
            }
        }

        protected void btnProcesarPago_Click(object sender, EventArgs e)
        {
            var errores = new List<string>();

            // Validar Nombre en tarjeta
            string nombre = txtCardName.Text.Trim();
            if (string.IsNullOrEmpty(nombre) || nombre.Length < 3)
            {
                errores.Add("Ingrese el nombre y apellido del titular tal como figura en la tarjeta.");
            }

            // Validar DNI
            string dni = txtCardDni.Text.Trim();
            if (string.IsNullOrEmpty(dni) || !Regex.IsMatch(dni, @"^\d{7,8}$"))
            {
                errores.Add("El DNI del titular debe tener entre 7 y 8 dígitos numéricos.");
            }

            // Validar Número de tarjeta (16 dígitos)
            string numeroTarjeta = txtCardNumber.Text.Replace(" ", "").Trim();
            if (string.IsNullOrEmpty(numeroTarjeta) || numeroTarjeta.Length != 16 || !Regex.IsMatch(numeroTarjeta, @"^\d{16}$"))
            {
                errores.Add("El número de tarjeta debe contener exactamente 16 dígitos numéricos.");
            }

            // Validar Vencimiento MM/AA
            string expiracion = txtExpiry.Text.Trim();
            bool expiryValida = false;
            if (Regex.IsMatch(expiracion, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                string[] partes = expiracion.Split('/');
                int mes = int.Parse(partes[0]);
                int anio = int.Parse("20" + partes[1]);
                DateTime ahora = DateTime.Now;

                if (anio > ahora.Year || (anio == ahora.Year && mes >= ahora.Month))
                {
                    expiryValida = true;
                }
            }

            if (!expiryValida)
            {
                errores.Add("La fecha de vencimiento debe tener formato MM/AA válido (mes entre 01-12) y no estar vencida.");
            }

            // Validar CVV
            string cvv = txtCvv.Text.Trim();
            if (string.IsNullOrEmpty(cvv) || !Regex.IsMatch(cvv, @"^\d{3,4}$"))
            {
                errores.Add("El código de seguridad (CVV) debe contener 3 o 4 dígitos numéricos.");
            }

            if (errores.Count > 0)
            {
                MostrarAlertaServidor("Atención: Por favor complete los datos obligatorios de la tarjeta:", errores);
                return;
            }

            // Procesar compra con tarjeta
            string cuotas = ddlCuotas.SelectedItem != null ? ddlCuotas.SelectedItem.Text : "1 Pago";
            string metodoDesc = $"Tarjeta de Crédito ({cuotas})";
            ProcesarTransaccionFinal(metodoDesc, $"Terminada en {numeroTarjeta.Substring(12)}");
        }

        protected void btnProcesarTransferencia_Click(object sender, EventArgs e)
        {
            var errores = new List<string>();

            // Validar Titular emisor
            string titular = txtTransferTitular.Text.Trim();
            if (string.IsNullOrEmpty(titular) || titular.Length < 3)
            {
                errores.Add("Debe ingresar el nombre del titular que realiza la transferencia bancaria.");
            }

            // Validar DNI / CUIT emisor
            string dni = txtTransferDni.Text.Trim();
            if (string.IsNullOrEmpty(dni) || !Regex.IsMatch(dni, @"^\d{7,11}$"))
            {
                errores.Add("El DNI o CUIT del titular emisor debe contener entre 7 y 11 dígitos numéricos.");
            }

            // Validar Banco de origen
            string banco = txtTransferBanco.Text.Trim();
            if (string.IsNullOrEmpty(banco) || banco.Length < 2)
            {
                errores.Add("Indique el banco o billetera digital desde la cual transfirió los fondos.");
            }

            // Validar CBU / CVU (22 dígitos)
            string cbu = txtTransferCbu.Text.Trim();
            if (string.IsNullOrEmpty(cbu) || cbu.Length != 22 || !Regex.IsMatch(cbu, @"^\d{22}$"))
            {
                errores.Add("El CBU o CVU de la cuenta de origen debe contener exactamente 22 dígitos numéricos.");
            }

            // Validar Comprobante / Referencia
            string comprobante = txtTransferComprobante.Text.Trim();
            if (string.IsNullOrEmpty(comprobante) || comprobante.Length < 4)
            {
                errores.Add("Ingrese el número de comprobante o referencia de la operación bancaria.");
            }

            // Validar Teléfono
            string tel = txtTransferTelefono.Text.Trim();
            if (string.IsNullOrEmpty(tel) || tel.Length < 8)
            {
                errores.Add("Ingrese un teléfono o WhatsApp de contacto válido.");
            }

            if (errores.Count > 0)
            {
                MostrarAlertaServidor("Atención: Por favor verifique los datos de la transferencia bancaria:", errores);
                return;
            }

            // Procesar compra con transferencia
            string metodoDesc = $"Transferencia Bancaria ({banco} - CBU: ...{cbu.Substring(18)})";
            ProcesarTransaccionFinal(metodoDesc, comprobante);
        }

        private void ProcesarTransaccionFinal(string metodoPago, string referencia)
        {
            try
            {
                int idAuto = int.Parse(Session["Checkout_AutoId"].ToString());
                AutoBase auto = _bllAuto.DevolverAuto(idAuto);

                if (auto == null)
                {
                    throw new Exception("El vehículo seleccionado no se encuentra disponible.");
                }

                string extras = Session["Checkout_Extras"]?.ToString() ?? string.Empty;

                Cliente cliente = ObtenerClienteActual();
                if (cliente == null)
                {
                    // Si el usuario navegó como visitante, instanciar y vincular cliente para la venta
                    string nombreCliente = !string.IsNullOrEmpty(txtCardName.Text) ? txtCardName.Text.Trim() : (!string.IsNullOrEmpty(txtTransferTitular.Text) ? txtTransferTitular.Text.Trim() : "Cliente RedLine");
                    string dniCliente = !string.IsNullOrEmpty(txtCardDni.Text) ? txtCardDni.Text.Trim() : (!string.IsNullOrEmpty(txtTransferDni.Text) ? txtTransferDni.Text.Trim() : "35000111");

                    cliente = new Cliente
                    {
                        ID = new Random().Next(100, 99999),
                        DNI = dniCliente,
                        Nombre = nombreCliente.Split(' ')[0],
                        Apellido = nombreCliente.Contains(" ") ? nombreCliente.Substring(nombreCliente.IndexOf(' ') + 1) : "Cliente",
                        Email = Session["UserEmail"]?.ToString() ?? "cliente@redlinecustoms.com",
                        Telefono = !string.IsNullOrEmpty(txtTransferTelefono.Text) ? txtTransferTelefono.Text.Trim() : "1155554444",
                        Direccion = "Av. Santa Fe 1234, CABA"
                    };
                    Session["ClienteSession"] = cliente;
                }

                // Registrar Venta y Factura
                Venta ventaRealizada = _bllVenta.RegistrarVenta(cliente, auto, extras, metodoPago, referencia);

                // Guardar datos en sesión para la página de confirmación y para el cliente
                Session["UltimaVenta"] = ventaRealizada;
                Session["UltimaFactura"] = ventaRealizada.Factura;
                Session["UltimaTransaccion"] = ventaRealizada.NumeroVenta;

                // Actualizar garage en sesión del cliente
                if (cliente.Garage == null) cliente.Garage = new List<AutoPersonalizado>();
                if (ventaRealizada.AutoPersonalizado != null && !cliente.Garage.Any(g => g.ID == ventaRealizada.AutoPersonalizado.ID))
                {
                    cliente.Garage.Add(ventaRealizada.AutoPersonalizado);
                }
                Session["ClienteSession"] = cliente;

                // Limpiar checkout activo
                Session["Checkout_AutoId"] = null;
                Session["Checkout_Extras"] = null;

                // Redirigir a página de pago exitoso
                Response.Redirect("PagoExitoso.aspx");
            }
            catch (Exception ex)
            {
                var errores = new List<string> { ex.Message };
                MostrarAlertaServidor("Error al procesar la operación:", errores);
            }
        }

        private Cliente ObtenerClienteActual()
        {
            if (Session["ClienteSession"] != null)
            {
                return (Cliente)Session["ClienteSession"];
            }

            if (SessionManager.Instancia.IsLogged())
            {
                var usuario = SessionManager.Instancia.Usuario;
                return new Cliente
                {
                    ID = usuario.ID,
                    DNI = usuario.DNI,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    Telefono = "1140003000",
                    Direccion = "Buenos Aires, Argentina"
                };
            }

            return null;
        }

        private void MostrarAlertaServidor(string titulo, List<string> errores)
        {
            string html = $"<strong><i class='fa-solid fa-triangle-exclamation'></i> {titulo}</strong><ul>";
            foreach (var err in errores)
            {
                html += $"<li>{err}</li>";
            }
            html += "</ul>";

            litMensajeAlerta.Text = html;
            divAlertaCheckout.Style["display"] = "block";

            string textoScript = string.Join("\\n• ", errores);
            string script = $"alert('{titulo.Replace("'", "\\'")}\\n\\n• {textoScript.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), script, true);
        }
    }
}
