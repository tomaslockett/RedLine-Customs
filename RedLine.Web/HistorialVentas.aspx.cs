using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Bll;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RedLine.Web
{
    public partial class HistorialVentas : BasePage
    {
        private readonly BLL_Venta _bllVenta = new BLL_Venta();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Acceso desde la gestión de clientes: "Ver perfil" prefiltra las ventas por DNI
                string dniDesdeQuery = Request.QueryString["dni"];
                if (!string.IsNullOrWhiteSpace(dniDesdeQuery))
                {
                    txtBuscar.Text = dniDesdeQuery.Trim();
                }

                CargarGrillaEIndicadores();
                CargarInformeMensual();
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarGrillaEIndicadores();
        }

        protected void btnGenerarInforme_Click(object sender, EventArgs e)
        {
            CargarInformeMensual();
        }

        protected void btnVerInformePdf_Click(object sender, EventArgs e)
        {
            RedirigirInformePdf("ver");
        }

        protected void btnDescargarInformePdf_Click(object sender, EventArgs e)
        {
            RedirigirInformePdf(null);
        }

        // =====================================================================
        // GRILLA DE VENTAS + KPIs GENERALES (comportamiento original)
        // =====================================================================

        /// <summary>
        /// Obtiene el listado de ventas desde BLL_Venta, aplica los filtros de búsqueda y fechas
        /// de forma simple, calcula los indicadores (KPIs) y enlaza la grilla dgvVentas.
        /// </summary>
        private void CargarGrillaEIndicadores()
        {
            try
            {
                List<Venta> todasLasVentas = _bllVenta.Listar();
                if (todasLasVentas == null)
                {
                    todasLasVentas = new List<Venta>();
                }

                string busqueda = txtBuscar.Text.Trim().ToLower();
                bool tieneFechaDesde = DateTime.TryParse(txtFechaDesde.Text, out DateTime fechaDesde);
                bool tieneFechaHasta = DateTime.TryParse(txtFechaHasta.Text, out DateTime fechaHasta);

                List<Venta> listaFiltrada = new List<Venta>();

                foreach (Venta v in todasLasVentas)
                {
                    if (v == null)
                    {
                        continue;
                    }

                    // Aseguramos objetos anidados no nulos para que Eval("Cliente.Nombre"), Eval("AutoBase.Marca")
                    // y Eval("AutoPersonalizado.NombreExtra") en el GridView no lancen excepción.
                    AsegurarDatosParaGrilla(v);

                    // 1. Filtro por texto (N° de venta, nombre/apellido del cliente o marca/modelo del vehículo)
                    if (!string.IsNullOrEmpty(busqueda))
                    {
                        string numVenta = (v.NumeroVenta ?? string.Empty).ToLower();
                        string nombreCliente = ((v.Cliente.Nombre ?? string.Empty) + " " + (v.Cliente.Apellido ?? string.Empty)).ToLower();
                        string dniCliente = (v.Cliente.DNI ?? string.Empty).ToLower();
                        string vehiculo = ((v.AutoBase.Marca ?? string.Empty) + " " + (v.AutoBase.Modelo ?? string.Empty)).ToLower();

                        bool coincideTexto = numVenta.Contains(busqueda) ||
                                             nombreCliente.Contains(busqueda) ||
                                             dniCliente.Contains(busqueda) ||
                                             vehiculo.Contains(busqueda);

                        if (!coincideTexto)
                        {
                            continue;
                        }
                    }

                    // 2. Filtro por Fecha Desde
                    if (tieneFechaDesde && v.Fecha.Date < fechaDesde.Date)
                    {
                        continue;
                    }

                    // 3. Filtro por Fecha Hasta
                    if (tieneFechaHasta && v.Fecha.Date > fechaHasta.Date)
                    {
                        continue;
                    }

                    listaFiltrada.Add(v);
                }

                // Cálculo de KPIs con bucle simple
                if (listaFiltrada.Count > 0)
                {
                    decimal ingresosTotales = 0m;
                    int ventasMesActual = 0;
                    int mesActual = DateTime.Now.Month;
                    int anioActual = DateTime.Now.Year;

                    foreach (Venta v in listaFiltrada)
                    {
                        ingresosTotales += v.Total;

                        if (v.Fecha.Month == mesActual && v.Fecha.Year == anioActual)
                        {
                            ventasMesActual++;
                        }
                    }

                    decimal promedio = ingresosTotales / listaFiltrada.Count;

                    lblVentasTotales.Text = listaFiltrada.Count.ToString();
                    lblIngresosTotales.Text = $"US$ {ingresosTotales:N2}";
                    lblTicketPromedio.Text = $"US$ {promedio:N2}";
                    lblVentasEsteMes.Text = ventasMesActual.ToString();
                }
                else
                {
                    lblVentasTotales.Text = "0";
                    lblIngresosTotales.Text = "US$ 0,00";
                    lblTicketPromedio.Text = "US$ 0,00";
                    lblVentasEsteMes.Text = "0";
                }

                dgvVentas.DataSource = listaFiltrada;
                dgvVentas.DataBind();
            }
            catch (Exception)
            {
                MostrarAlertaTraducida("msg_error_cargar_ventas");
            }
        }

        /// <summary>
        /// Garantiza que una Venta tenga instancias válidas de Cliente, AutoBase y AutoPersonalizado
        /// antes de enlazarla al GridView.
        /// </summary>
        private void AsegurarDatosParaGrilla(Venta v)
        {
            if (v.Cliente == null)
            {
                v.Cliente = new Cliente
                {
                    Nombre = "Consumidor",
                    Apellido = "Final",
                    DNI = "-"
                };
            }

            if (v.AutoBase == null)
            {
                if (v.AutoPersonalizado != null && v.AutoPersonalizado.AuBase != null)
                {
                    v.AutoBase = v.AutoPersonalizado.AuBase;
                }
                else
                {
                    v.AutoBase = new AutoBase
                    {
                        Marca = "RedLine",
                        Modelo = "Custom Edition"
                    };
                }
            }

            if (v.AutoPersonalizado == null)
            {
                v.AutoPersonalizado = new AutoPersonalizado
                {
                    AuBase = v.AutoBase,
                    NombreExtra = Traducir("texto_estandar_base")
                };
            }
            else if (string.IsNullOrEmpty(v.AutoPersonalizado.NombreExtra) || v.AutoPersonalizado.NombreExtra == TextosComercialesVentas.ConfiguracionEstandar)
            {
                v.AutoPersonalizado.NombreExtra = Traducir("texto_estandar_base");
            }
        }

        // =====================================================================
        // INFORME MENSUAL DE VENTAS
        // =====================================================================

        private List<Venta> ObtenerTodasLasVentasSeguras()
        {
            try
            {
                return _bllVenta.Listar() ?? new List<Venta>();
            }
            catch
            {
                return new List<Venta>();
            }
        }

        /// <summary>
        /// Pobla los selectores de Mes y Año con los años existentes en las ventas más el año
        /// actual, y preselecciona por defecto el último período con actividad (o el actual).
        /// Los textos de los meses traducen por valor vía BasePage (claves ddlInformeMes_01..12).
        /// </summary>
        private void CargarSelectoresPeriodo(List<Venta> ventas, DateTime referenciaRango)
        {
            if (ddlInformeMes.Items.Count == 0)
            {
                for (int m = 1; m <= 12; m++)
                {
                    ddlInformeMes.Items.Add(new ListItem(m.ToString("00"), m.ToString("00")));
                }
            }

            if (ddlInformeAnio.Items.Count == 0)
            {
                List<int> anios = ventas.Select(v => v.Fecha.Year).Distinct().ToList();
                if (!anios.Contains(DateTime.Now.Year))
                {
                    anios.Add(DateTime.Now.Year);
                }
                anios.Sort((a, b) => b.CompareTo(a));

                foreach (int anio in anios)
                {
                    ddlInformeAnio.Items.Add(new ListItem(anio.ToString(), anio.ToString()));
                }
            }

            if (!IsPostBack || string.IsNullOrEmpty(ddlInformeAnio.SelectedValue) || !anioExisteEnLista(referenciaRango.Year))
            {
                ddlInformeAnio.SelectedValue = referenciaRango.Year.ToString();
            }

            if (!IsPostBack)
            {
                ddlInformeMes.SelectedValue = referenciaRango.Month.ToString("00");
            }
        }

        private bool anioExisteEnLista(int anio)
        {
            return ddlInformeAnio.Items.FindByValue(anio.ToString()) != null;
        }

        private bool MesSeleccionado(out int mes)
        {
            return int.TryParse(ddlInformeMes.SelectedValue, out mes) && mes >= 1 && mes <= 12;
        }

        private bool AnioSeleccionado(out int anio)
        {
            return int.TryParse(ddlInformeAnio.SelectedValue, out anio) && anio >= 1900 && anio <= 2100;
        }

        /// <summary>
        /// Carga el informe mensual: selectores, KPIs del período, los 4 gráficos SVG
        /// y el encabezado corporativo. Muestra estado vacío si el período no tiene ventas.
        /// </summary>
        private void CargarInformeMensual()
        {
            try
            {
                List<Venta> ventas = ObtenerTodasLasVentasSeguras();
                DateTime referenciaRango = ventas.Count > 0 ? ventas.Max(v => v.Fecha) : DateTime.Now;

                CargarSelectoresPeriodo(ventas, referenciaRango);

                int mes;
                int anio;
                if (!MesSeleccionado(out mes)) mes = DateTime.Now.Month;
                if (!AnioSeleccionado(out anio)) anio = DateTime.Now.Year;

                var datos = InformeVentasServicio.Calcular(ventas, mes, anio);

                if (datos == null || datos.Unidades == 0)
                {
                    phInformeDatos.Visible = false;
                    phInformeVacio.Visible = true;
                    return;
                }

                lblInformeValorPeriodo.Text = TraducirSeguro(
                    "ddlInformeMes_" + datos.Mes.ToString("00"),
                    NombreMesDefecto(datos.Mes)) + " " + datos.Anio;
                lblInformeValorFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                phInformeVacio.Visible = false;
                phInformeDatos.Visible = true;

                // Encabezado corporativo + KPIs del período
                lblValorUnidadesMes.Text = datos.Unidades.ToString();
                lblValorFacturacionMes.Text = string.Format("US$ {0:N2}", datos.TotalFacturado);
                lblValorTicketPromedioMes.Text = string.Format("US$ {0:N2}", datos.TicketPromedio);
                lblValorSubtotalNetoMes.Text = string.Format("US$ {0:N2}", datos.SubtotalNeto);
                lblValorIvaMes.Text = string.Format("US$ {0:N2} ({1:0}%)", datos.MontoIva, datos.TotalFacturado > 0 ? 100m * datos.MontoIva / datos.TotalFacturado : 0m);
                lblValorPersonalizadosMes.Text = $"{datos.CantidadPersonalizados} / {datos.CantidadEstandar} ({datos.PorcentajePersonalizados:0}%)";
                lblValorMarcaDestacadaMes.Text = datos.MarcaDestacada ?? "-";

                // Gráficos SVG (server-side, sin librerías externas)
                ltGraficoEvolucionDiaria.Text = GenerarSvgBarras(
                    datos.SerieDiaria.Select(d => new InformeVentasServicio.CategoriaVolumen
                    {
                        Etiqueta = d.Dia.ToString(),
                        Monto = d.Monto
                    }).ToList(),
                    string.Format("{0} · {1}: {2}",
                        TraducirSeguro("lblLeyendaFacturacion", "Facturación (US$)"),
                        TraducirSeguro("lblLeyendaOperaciones", "Operaciones"),
                        datos.SerieDiaria.Sum(d => d.Operaciones)),
                    false,
                    false);

                ltGraficoVentasMarca.Text = GenerarSvgBarras(
                    datos.PorVehiculo.Take(7).ToList(),
                    null,
                    true,
                    true);

                ltGraficoPersonalizadosEstandar.Text = GenerarSvgDona(
                    datos.CantidadPersonalizados,
                    datos.CantidadEstandar,
                    datos.MontoPersonalizados,
                    datos.MontoEstandar,
                    TraducirSeguro("texto_personalizado_informe", "Personalizado"),
                    TraducirSeguro("texto_estandar_base", "Estándar de Fábrica"));

                ltGraficoMetodosPago.Text = GenerarSvgBarrasHorizontales(datos.PorMetodoPago.Take(7).ToList());
            }
            catch (Exception)
            {
                phInformeDatos.Visible = false;
                phInformeVacio.Visible = false;
                MostrarAlertaTraducida("msg_informe_error_generar");
            }
        }

        private void RedirigirInformePdf(string modo)
        {
            int mes;
            int anio;
            if (!MesSeleccionado(out mes)) mes = DateTime.Now.Month;
            if (!AnioSeleccionado(out anio)) anio = DateTime.Now.Year;

            // Pre-validación del período: si no hay ventas mostramos la alerta traducida
            // en la misma página en lugar de navegar a la respuesta 404 del handler.
            List<Venta> ventasPreliminares = ObtenerTodasLasVentasSeguras();
            InformeVentasServicio.DatosInformeVentas preliminar = InformeVentasServicio.Calcular(ventasPreliminares, mes, anio);
            if (preliminar == null || preliminar.Unidades == 0)
            {
                MostrarAlertaTraducida("msg_informe_sin_datos");
                return;
            }

            string url = $"~/DescargarInformeVentas.ashx?mes={mes}&anio={anio}";
            if (!string.IsNullOrEmpty(modo))
            {
                url += "&modo=" + modo;
            }

            Response.Redirect(url, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Nombre de mes en español por defecto (respaldo legible cuando la clave
        /// ddlInformeMes_XX aún no está sembrada en la base de datos).
        /// </summary>
        private static string NombreMesDefecto(int mes)
        {
            int mesSeguro = Math.Max(1, Math.Min(12, mes));
            string nombre = new DateTime(2000, mesSeguro, 1)
                .ToString("MMMM", System.Globalization.CultureInfo.GetCultureInfo("es"));
            return char.ToUpper(nombre[0]) + nombre.Substring(1);
        }

        // =====================================================================
        // RENDERIZADO SVG (server-side, sin librerías ni CDN)
        // =====================================================================

        private const string ColorAcento = "#D93416";
        private const string ColorAcentoOscuro = "#B32A12";
        private const string ColorBarra = "#3A4453";
        private const string ColorCuadricula = "#232C3B";
        private const string ColorEje = "#5A6478";
        private const string ColorLeyenda = "#8A93A6";
        private const string ColorPanel = "#161D29";

        private static string EscXml(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            return texto
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("'", "&apos;")
                .Replace("\"", "&quot;");
        }

        private static string FormatoMontoCorto(decimal valor)
        {
            decimal absoluto = Math.Abs(valor);
            if (absoluto >= 1000000m) return "$" + string.Format("{0:0.##}", valor / 1000000m) + "M";
            if (absoluto >= 1000m) return "$" + string.Format("{0:0.##}", valor / 1000m) + "k";
            return "$" + string.Format("{0:N0}", valor);
        }

        /// <summary>
        /// Gráfico de barras verticales (evolución diaria / marcas-modelos).
        /// </summary>
        private static string GenerarSvgBarras(List<InformeVentasServicio.CategoriaVolumen> datos, string leyenda, bool truncarEtiquetas, bool mostrarValores)
        {
            const int ancho = 640;
            const int alto = 300;
            const float margenIzq = 58f;
            const float margenSup = 24f;
            const float margenInf = 36f;
            const float margenDer = 14f;

            float plotAncho = ancho - margenIzq - margenDer;
            float plotAlto = alto - margenSup - margenInf;
            float baseline = alto - margenInf;

            var sb = new StringBuilder();

            decimal max = datos != null && datos.Count > 0 ? datos.Max(d => d.Monto) : 0m;
            if (max <= 0) max = 1m;

            if (datos == null || datos.Count == 0)
            {
                sb.Append($"<svg viewBox='0 0 {ancho} {alto}' xmlns='http://www.w3.org/2000/svg' class='informe-svg' role='img'>");
                sb.Append($"<text x='{ancho / 2}' y='{alto / 2}' fill='{ColorLeyenda}' font-size='13' text-anchor='middle'>-</text>");
                sb.Append("</svg>");
                return sb.ToString();
            }

            sb.Append($"<svg viewBox='0 0 {ancho} {alto}' xmlns='http://www.w3.org/2000/svg' class='informe-svg' role='img'>");

            // Leyenda (arriba, anclada a la derecha)
            if (!string.IsNullOrEmpty(leyenda))
            {
                sb.Append($"<text x='{ancho - margenDer}' y='15' fill='{ColorLeyenda}' font-size='10.5' text-anchor='end'>{EscXml(leyenda)}</text>");
            }

            // Grillas del eje Y (0 / 50% / 100%)
            foreach (decimal fraccion in new decimal[] { 0.5m, 1m })
            {
                float y = baseline - plotAlto * (float)(fraccion / 1m);
                sb.Append($"<line x1='{margenIzq}' y1='{Fmt(y)}' x2='{ancho - margenDer}' y2='{Fmt(y)}' stroke='{ColorCuadricula}' stroke-width='1'/>");
                sb.Append($"<text x='{margenIzq - 6}' y='{Fmt(y + 3)}' fill='{ColorLeyenda}' font-size='9.5' text-anchor='end'>{EscXml(FormatoMontoCorto(max * fraccion))}</text>");
            }

            // Eje X (baseline)
            sb.Append($"<line x1='{margenIzq}' y1='{Fmt(baseline)}' x2='{ancho - margenDer}' y2='{Fmt(baseline)}' stroke='{ColorEje}' stroke-width='1.4'/>");

            float slot = plotAncho / datos.Count;
            float anchoBarra = slot * 0.62f;
            if (anchoBarra > 28f) anchoBarra = 28f;

            bool mostrarEtiqueta = datos.Count <= 16;

            for (int i = 0; i < datos.Count; i++)
            {
                var dato = datos[i];
                float altura = (float)(dato.Monto / max) * plotAlto;
                float bx = margenIzq + slot * i + (slot - anchoBarra) / 2f;
                float by = baseline - Math.Max(altura, 1f);
                string relleno = dato.Monto == max ? ColorAcento : ColorBarra;

                sb.Append($"<rect x='{Fmt(bx)}' y='{Fmt(by)}' width='{Fmt(anchoBarra)}' height='{Fmt(Math.Max(altura, 1f))}' rx='2.5' fill='{relleno}'/>");

                if (mostrarValores && datos.Count <= 8 && altura > 12f)
                {
                    sb.Append($"<text x='{Fmt(bx + anchoBarra / 2f)}' y='{Fmt(by - 5)}' fill='#FFFFFF' font-size='9.5' font-weight='600' text-anchor='middle'>{EscXml(FormatoMontoCorto(dato.Monto))}</text>");
                }

                if (mostrarEtiqueta && (i % 2 == 0 || datos.Count <= 16))
                {
                    string etiqueta = dato.Etiqueta ?? "-";
                    if (truncarEtiquetas && etiqueta.Length > 15) etiqueta = etiqueta.Substring(0, 14) + "…";
                    sb.Append($"<text x='{Fmt(margenIzq + slot * i + slot / 2f)}' y='{Fmt(baseline + 13)}' fill='{ColorLeyenda}' font-size='9' text-anchor='middle'>{EscXml(etiqueta)}</text>");
                }
                
                sb.Append($"<rect x='{Fmt(bx)}' y='{Fmt(baseline)}' width='{Fmt(anchoBarra)}' height='4' fill='{ColorAcentoOscuro}' opacity='0.55'/>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        /// <summary>
        /// Gráfico de dona: proporción de ventas personalizadas vs. estándar de fábrica.
        /// </summary>
        private static string GenerarSvgDona(int personalizados, int estandar, decimal montoPersonalizados, decimal montoEstandar, string etiquetaPersonalizados, string etiquetaEstandar)
        {
            const int ancho = 640;
            const int alto = 300;
            const float cx = 150f;
            const float cy = 160f;
            const float radio = 78f;
            const float grosor = 40f;

            int total = personalizados + estandar;
            decimal pctPersonalizados = total > 0 ? Math.Round(100m * personalizados / total, 1) : 0m;
            decimal pctEstandar = total > 0 ? 100m - pctPersonalizados : 0m;
            double circunferencia = 2 * Math.PI * radio;
            double longitudArco = total > 0 ? circunferencia * (personalizados / (double)total) : 0d;

            var sb = new StringBuilder();
            sb.Append($"<svg viewBox='0 0 {ancho} {alto}' xmlns='http://www.w3.org/2000/svg' class='informe-svg' role='img'>");
            sb.Append($"<circle cx='{cx}' cy='{cy}' r='{radio}' fill='none' stroke='{ColorPanel}' stroke-width='{grosor}'/>");
            sb.Append($"<circle cx='{cx}' cy='{cy}' r='{radio}' fill='none' stroke='{ColorBarra}' stroke-width='{grosor}' stroke-dasharray='{Fmt(circunferencia)} {Fmt(circunferencia)}'/>");
            if (longitudArco > 0.5d)
            {
                sb.Append($"<circle cx='{cx}' cy='{cy}' r='{radio}' fill='none' stroke='{ColorAcento}' stroke-width='{grosor}' transform='rotate(-90 {cx} {cy})' stroke-dasharray='{Fmt(longitudArco)} {Fmt(circunferencia - longitudArco)}'/>");
            }

            sb.Append($"<text x='{cx}' y='{cy - 2}' fill='#FFFFFF' font-size='30' font-weight='800' text-anchor='middle'>{total}</text>");
            sb.Append($"<text x='{cx}' y='{cy + 20}' fill='{ColorLeyenda}' font-size='11' text-anchor='middle'>{EscXml(string.Format("US$ {0:N0}", montoPersonalizados + montoEstandar))}</text>");

            float leyendaX = 310f;
            float leyendaY = cy - 36f;

            // Personalizados
            sb.Append($"<rect x='{Fmt(leyendaX)}' y='{Fmt(leyendaY - 10)}' width='14' height='14' rx='3' fill='{ColorAcento}'/>");
            sb.Append($"<text x='{Fmt(leyendaX + 22)}' y='{Fmt(leyendaY + 2)}' fill='#FFFFFF' font-size='12.5' font-weight='600'>{EscXml(etiquetaPersonalizados)}: {personalizados} ({EscXml(pctPersonalizados.ToString("0.#"))}%)</text>");
            sb.Append($"<text x='{Fmt(leyendaX + 22)}' y='{Fmt(leyendaY + 18)}' fill='{ColorLeyenda}' font-size='10.5'>{EscXml(string.Format("US$ {0:N0}", montoPersonalizados))}</text>");

            // Estándar de fábrica
            sb.Append($"<rect x='{Fmt(leyendaX)}' y='{Fmt(leyendaY + 44)}' width='14' height='14' rx='3' fill='{ColorBarra}'/>");
            sb.Append($"<text x='{Fmt(leyendaX + 22)}' y='{Fmt(leyendaY + 56)}' fill='#FFFFFF' font-size='12.5' font-weight='600'>{EscXml(etiquetaEstandar)}: {estandar} ({EscXml(pctEstandar.ToString("0.#"))}%)</text>");
            sb.Append($"<text x='{Fmt(leyendaX + 22)}' y='{Fmt(leyendaY + 74)}' fill='{ColorLeyenda}' font-size='10.5'>{EscXml(string.Format("US$ {0:N0}", montoEstandar))}</text>");

            sb.Append("</svg>");
            return sb.ToString();
        }

        /// <summary>
        /// Gráfico de barras horizontales (métodos de pago).
        /// </summary>
        private static string GenerarSvgBarrasHorizontales(List<InformeVentasServicio.CategoriaVolumen> datos)
        {
            const int ancho = 640;

            if (datos == null || datos.Count == 0)
            {
                return $"<svg viewBox='0 0 {ancho} 80' xmlns='http://www.w3.org/2000/svg' class='informe-svg'><text x='{ancho / 2}' y='45' fill='{ColorLeyenda}' font-size='12' text-anchor='middle'>-</text></svg>";
            }

            int alto = 30 + datos.Count * 44 + 10;
            var sb = new StringBuilder();
            sb.Append($"<svg viewBox='0 0 {ancho} {alto}' xmlns='http://www.w3.org/2000/svg' class='informe-svg' role='img'>");

            decimal max = datos.Max(d => d.Monto);
            if (max <= 0) max = 1m;
            decimal total = datos.Sum(d => d.Monto);

            const float etiquetaX = 140f;
            const float barraX = 152f;
            const float barraAnchoMax = 348f;

            for (int i = 0; i < datos.Count; i++)
            {
                var dato = datos[i];
                float cy = 34f + i * 44f;
                decimal pct = total > 0 ? 100m * dato.Monto / total : 0m;
                float largo = (float)(dato.Monto / max) * barraAnchoMax;
                string relleno = i == 0 ? ColorAcento : ColorBarra;

                sb.Append($"<text x='{etiquetaX}' y='{Fmt(cy + 13)}' fill='#FFFFFF' font-size='11.5' text-anchor='end'>{EscXml(TruncarTexto(dato.Etiqueta, 20))}</text>");
                sb.Append($"<rect x='{barraX}' y='{Fmt(cy)}' width='{Fmt(Math.Max(largo, 3f))}' height='20' rx='4' fill='{relleno}'/>");
                sb.Append($"<text x='{Fmt(barraX + barraAnchoMax + 14)}' y='{Fmt(cy + 9)}' fill='#FFFFFF' font-size='10.5' font-weight='600'>{EscXml(string.Format("US$ {0:N0}", dato.Monto))}</text>");
                sb.Append($"<text x='{Fmt(barraX + barraAnchoMax + 14)}' y='{Fmt(cy + 21)}' fill='{ColorLeyenda}' font-size='9.5'>{EscXml(pct.ToString("0.#"))}%  ·  {dato.Unidades} ops</text>");
            }

            sb.Append("</svg>");
            return sb.ToString();
        }

        private static string TruncarTexto(string texto, int max)
        {
            if (string.IsNullOrEmpty(texto)) return "-";
            return texto.Length <= max ? texto : texto.Substring(0, max - 1) + "…";
        }

        private static string Fmt(double valor)
        {
            return valor.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
