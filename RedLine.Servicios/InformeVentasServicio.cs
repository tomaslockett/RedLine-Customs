using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RedLine.Servicios
{
    /// <summary>
    /// Servicio del Informe Mensual de Ventas. Calcula todas las métricas y agrupaciones
    /// del período seleccionado a partir del listado ya existente (BLL_Venta.Listar()),
    /// sin agregar código nuevo en BLL/BE/DAL, y genera el PDF nativo en memoria
    /// (%PDF-1.4, ISO-8859-1, fuentes Helvetica estándar) replicando la técnica de
    /// PdfFacturaServicio.cs pero con soporte de múltiples páginas y gráficos vectoriales.
    /// Todos los textos salen en el idioma activo del usuario (SubjectIdioma).
    /// </summary>
    public static class InformeVentasServicio
    {
        private const string RojoRedline = "0.812 0.165 0.114";   // #D93416
        private const string AzulDarkline = "0.058 0.078 0.11";   // #0F141C
        private const string GrisTexto = "0.35 0.35 0.4";
        private const string PanelClaro = "0.96 0.96 0.975";
        private const string BordePanel = "0.68 0.68 0.72";

        private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

        /// <summary>
        /// Traducción segura para el PDF: si la clave aún no está sembrada en la base de
        /// datos (vacía o "[clave]") devuelve el texto español por defecto para que el
        /// documento jamás muestre corchetes "[clave]" al usuario.
        /// </summary>
        private static string Tr(string clave, string predeterminado)
        {
            string texto = SubjectIdioma.Instancia.Traducir(clave);
            if (string.IsNullOrWhiteSpace(texto) || (texto.StartsWith("[") && texto.EndsWith("]")))
            {
                return predeterminado;
            }
            return texto;
        }

        // =====================================================================
        // DTO DEL INFORME
        // =====================================================================

        public class PuntoSerie
        {
            public int Dia { get; set; }
            public decimal Monto { get; set; }
            public int Operaciones { get; set; }
        }

        public class CategoriaVolumen
        {
            public string Etiqueta { get; set; }
            public int Unidades { get; set; }
            public decimal Monto { get; set; }
        }

        public class DatosInformeVentas
        {
            public int Mes { get; set; }
            public int Anio { get; set; }
            public int DiasDelMes { get; set; }
            public List<Venta> VentasDelMes { get; set; }

            public int Unidades
            {
                get { return VentasDelMes != null ? VentasDelMes.Count : 0; }
            }

            public decimal SubtotalNeto { get; set; }
            public decimal MontoIva { get; set; }
            public decimal TotalFacturado { get; set; }
            public decimal TicketPromedio { get; set; }

            public List<PuntoSerie> SerieDiaria { get; set; }
            public List<CategoriaVolumen> PorVehiculo { get; set; }
            public List<CategoriaVolumen> PorMetodoPago { get; set; }

            public int CantidadPersonalizados { get; set; }
            public int CantidadEstandar { get; set; }
            public decimal MontoPersonalizados { get; set; }
            public decimal MontoEstandar { get; set; }

            public string MarcaDestacada { get; set; }

            public decimal PorcentajePersonalizados
            {
                get
                {
                    int total = CantidadPersonalizados + CantidadEstandar;
                    return total == 0 ? 0m : Math.Round(CantidadPersonalizados * 100m / total, 0);
                }
            }
        }

        // =====================================================================
        // CALCULO DE METRICAS (consume únicamente RedLine.Be)
        // =====================================================================

        public static DatosInformeVentas Calcular(IEnumerable<Venta> todasLasVentas, int mes, int anio)
        {
            var informe = new DatosInformeVentas
            {
                Mes = mes,
                Anio = anio,
                DiasDelMes = DateTime.DaysInMonth(anio, mes),
                VentasDelMes = new List<Venta>(),
                SerieDiaria = new List<PuntoSerie>(),
                PorVehiculo = new List<CategoriaVolumen>(),
                PorMetodoPago = new List<CategoriaVolumen>()
            };

            if (todasLasVentas == null)
            {
                return informe;
            }

            var porDia = new Dictionary<int, PuntoSerie>();
            var porVehiculo = new Dictionary<string, CategoriaVolumen>();
            var porMetodoPago = new Dictionary<string, CategoriaVolumen>();

            decimal subtotal = 0m, iva = 0m, total = 0m;

            foreach (Venta v in todasLasVentas)
            {
                if (v == null || v.Fecha.Year != anio || v.Fecha.Month != mes)
                {
                    continue;
                }

                informe.VentasDelMes.Add(v);

                decimal totalVenta = v.Total;
                decimal subtotalVenta = v.Subtotal;
                decimal ivaVenta = totalVenta - subtotalVenta;

                subtotal += subtotalVenta;
                iva += ivaVenta;
                total += totalVenta;

                // Serie diaria (evolución de ingresos/operaciones)
                PuntoSerie punto;
                if (!porDia.TryGetValue(v.Fecha.Day, out punto))
                {
                    punto = new PuntoSerie { Dia = v.Fecha.Day };
                    porDia.Add(v.Fecha.Day, punto);
                }
                punto.Monto += totalVenta;
                punto.Operaciones++;

                // Agrupación por marca/modelo base del vehículo
                string etiquetaVehiculo = EtiquetaVehiculo(v);
                CategoriaVolumen categoriaVehiculo;
                if (!porVehiculo.TryGetValue(etiquetaVehiculo, out categoriaVehiculo))
                {
                    categoriaVehiculo = new CategoriaVolumen { Etiqueta = etiquetaVehiculo };
                    porVehiculo.Add(etiquetaVehiculo, categoriaVehiculo);
                }
                categoriaVehiculo.Unidades++;
                categoriaVehiculo.Monto += totalVenta;

                // Agrupación por método de pago
                string metodo = MetodoPagoDeVenta(v);
                CategoriaVolumen categoriaPago;
                if (!porMetodoPago.TryGetValue(metodo, out categoriaPago))
                {
                    categoriaPago = new CategoriaVolumen { Etiqueta = metodo };
                    porMetodoPago.Add(metodo, categoriaPago);
                }
                categoriaPago.Unidades++;
                categoriaPago.Monto += totalVenta;

                // Personalizados vs. estándar de fábrica
                if (v.AutoPersonalizado != null)
                {
                    informe.CantidadPersonalizados++;
                    informe.MontoPersonalizados += totalVenta;
                }
                else
                {
                    informe.CantidadEstandar++;
                    informe.MontoEstandar += totalVenta;
                }
            }

            informe.SubtotalNeto = subtotal;
            informe.MontoIva = iva;
            informe.TotalFacturado = total;
            informe.TicketPromedio = informe.Unidades > 0 ? total / informe.Unidades : 0m;

            informe.SerieDiaria = porDia.OrderBy(d => d.Key).Select(d => d.Value).ToList();
            informe.PorVehiculo = porVehiculo.Values
                .OrderByDescending(c => c.Monto)
                .ThenByDescending(c => c.Unidades)
                .ToList();
            informe.PorMetodoPago = porMetodoPago.Values
                .OrderByDescending(c => c.Monto)
                .ThenBy(c => c.Etiqueta)
                .ToList();

            var destacado = informe.PorVehiculo.FirstOrDefault();
            informe.MarcaDestacada = destacado == null
                ? "-"
                : destacado.Etiqueta + " (" + destacado.Unidades + ")";

            return informe;
        }

        public static string EtiquetaVehiculo(Venta v)
        {
            if (v == null) return "-";
            var autoBase = v.AutoBase ?? (v.AutoPersonalizado != null ? v.AutoPersonalizado.AuBase : null);
            if (autoBase == null) return "RedLine Custom Edition";
            return (autoBase.Marca + " " + autoBase.Modelo).Trim();
        }

        public static string MetodoPagoDeVenta(Venta v)
        {
            if (v == null) return "-";
            if (!string.IsNullOrWhiteSpace(v.MetodoPago)) return v.MetodoPago.Trim();
            if (v.Factura != null && !string.IsNullOrWhiteSpace(v.Factura.MetodoPago)) return v.Factura.MetodoPago.Trim();
            return "-";
        }

        /// <summary>Nombre del mes traducido reutilizando las claves ddlInformeMes_01..12.</summary>
        public static string NombreMesTraducido(int mes)
        {
            string texto = SubjectIdioma.Instancia.Traducir("ddlInformeMes_" + mes.ToString("00", Cultura));

            // Respaldo: si la clave no está sembrada, usamos el nombre de mes en español
            // generado dinámicamente para no renderizar "[ddlInformeMes_XX]" en el PDF.
            if (string.IsNullOrWhiteSpace(texto) || (texto.StartsWith("[") && texto.EndsWith("]")))
            {
                string nombre = new DateTime(2000, Math.Max(1, Math.Min(12, mes)), 1)
                    .ToString("MMMM", CultureInfo.GetCultureInfo("es"));
                return char.ToUpper(nombre[0]) + nombre.Substring(1);
            }

            return texto;
        }

        private static string Moneda(decimal valor)
        {
            return "US$ " + valor.ToString("N2", Cultura);
        }

        private static string FormatoCorto(decimal valor)
        {
            decimal absoluto = Math.Abs(valor);
            if (absoluto >= 1000000m) return "$" + (valor / 1000000m).ToString("0.##", Cultura) + "M";
            if (absoluto >= 1000m) return "$" + (valor / 1000m).ToString("0.##", Cultura) + "k";
            return "$" + valor.ToString("N0", Cultura);
        }

        // =====================================================================
        // PDF NATIVO 1.4 MULTI-PAGINA
        // =====================================================================

        public static byte[] GenerarPdfInformeVentas(DatosInformeVentas informe)
        {
            if (informe == null) throw new ArgumentNullException("informe");
            if (informe.VentasDelMes == null) throw new ArgumentNullException("informe.VentasDelMes");

            string nombreMes = NombreMesTraducido(informe.Mes);
            string periodo = nombreMes + " " + informe.Anio;
            string fechaEmision = DateTime.Now.ToString("dd/MM/yyyy HH:mm", Cultura);

            const int filasPorPagina = 26;
            int paginasDetalle = Math.Max(1, (int)Math.Ceiling(informe.Unidades / (double)filasPorPagina));
            int totalPaginas = 3 + paginasDetalle;

            List<byte[]> paginas = new List<byte[]>();
            paginas.Add(GenerarPaginaEncabezado(informe, periodo, fechaEmision, totalPaginas));
            paginas.Add(GenerarPaginaGraficos1(informe, periodo, totalPaginas));
            paginas.Add(GenerarPaginaGraficos2(informe, totalPaginas));

            for (int p = 0; p < paginasDetalle; p++)
            {
                paginas.Add(GenerarPaginaDetalle(informe, periodo, p, filasPorPagina, totalPaginas));
            }

            return ConstruirPdf(paginas);
        }

        // ---------- Página 1: encabezado corporativo + KPIs ----------

        private static byte[] GenerarPaginaEncabezado(DatosInformeVentas informe, string periodo, string fechaEmision, int totalPaginas)
        {
            var sb = new StringBuilder();

            sb.AppendLine("q");
            sb.AppendLine("1 1 1 rg 0 0 595.28 841.89 re f");

            // Banda roja institucional (misma técnica de PdfFacturaServicio)
            sb.AppendLine("0.812 0.165 0.114 rg 0 760 595.28 82 re f");
            sb.AppendLine("0.1 0.1 0.1 RG 2 w 0 760 m 595.28 760 l S");

            AgregarTexto(sb, 35, 800, 22, true, "1 1 1", "REDLINE CUSTOMS");
            AgregarTexto(sb, 35, 784, 8.5f, false, "0.95 0.95 0.95", Tr("lblInformeEmpresaSubtitulo", "Concesionaria y Taller de Personalización de Alta Gama"));
            AgregarTexto(sb, 356, 806, 14, true, "1 1 1", Tr("pdfInformeTitulo", "Informe Mensual de Ventas"));
            AgregarTexto(sb, 356, 790, 9, false, "1 1 1", periodo.ToUpper(Cultura));
            sb.AppendLine("Q");

            // Recuadro de datos corporativos
            sb.AppendLine("q");
            sb.AppendLine("0.97 0.97 0.98 rg 35 660 525 82 re f");
            sb.AppendLine("0.8 0.8 0.85 RG 1 w 35 660 525 82 re S");
            sb.AppendLine("0.8 0.8 0.85 RG 1 w 297.64 660 m 297.64 742 l S");

            AgregarTexto(sb, 45, 726, 10, true, "0.15 0.15 0.15", "REDLINE CUSTOMS S.A.");
            AgregarTexto(sb, 45, 712, 8, false, GrisTexto, "CUIT: 30-71829384-9 | IVA Responsable Inscripto");
            AgregarTexto(sb, 45, 699, 8, false, GrisTexto, "Domicilio: Av. del Libertador 4500, CABA");
            AgregarTexto(sb, 45, 686, 8, false, GrisTexto, "www.redlinecustoms.com");

            AgregarTexto(sb, 310, 726, 11, true, RojoRedline, periodo.ToUpper(Cultura));
            AgregarTexto(sb, 310, 710, 8, false, GrisTexto, Tr("lblInformePeriodo", "Período:") + " " + periodo);
            AgregarTexto(sb, 310, 697, 8, false, GrisTexto, Tr("lblInformeFechaEmision", "Fecha de emisión:") + " " + fechaEmision);
            AgregarTexto(sb, 310, 684, 8, false, RojoRedline, Tr("pdfTextoCantVentas", "Cantidad de ventas:") + " " + informe.Unidades);
            sb.AppendLine("Q");

            // Tarjetas de resumen ejecutivo (3 x 2)
            string[][] kpis = new string[][]
            {
                new string[] { Tr("pdfTextoUnidades", "Unidades Vendidas"), informe.Unidades.ToString(Cultura) },
                new string[] { Tr("pdfTextoFacturacion", "Facturación Total"), Moneda(informe.TotalFacturado) },
                new string[] { Tr("lblTextoSubtotalNeto", "Subtotal Neto:"), Moneda(informe.SubtotalNeto) },
                new string[] { Tr("lblTextoIva21", "IVA Liquidado (21%):"), Moneda(informe.MontoIva) },
                new string[] { Tr("pdfTextoTicketPromedio", "Ticket Promedio"), Moneda(informe.TicketPromedio) },
                new string[] { Tr("pdfTextoMarcaDestacada", "Marca / Modelo Destacado"), informe.MarcaDestacada ?? "-" }
            };

            float[] columnasKpi = { 35f, 215f, 395f };
            for (int i = 0; i < kpis.Length; i++)
            {
                float kx = columnasKpi[i % 3];
                float ky = i < 3 ? 492f : 412f;
                DibujarTarjetaKpi(sb, kx, ky, 165f, 68f, kpis[i][0], kpis[i][1]);
            }

            AgregarPiePagina(sb, totalPaginas, 1);
            return EncodingBytes(sb);
        }

        // ---------- Página 2: evolución diaria + marcas ----------

        private static byte[] GenerarPaginaGraficos1(DatosInformeVentas informe, string periodo, int totalPaginas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("q");
            sb.AppendLine("1 1 1 rg 0 0 595.28 841.89 re f");

            int operaciones = informe.SerieDiaria.Sum(d => d.Operaciones);

            AgregarTexto(sb, 35, 806, 12, true, AzulDarkline, Tr("lblGraficoEvolucionDiaria", "Evolución Diaria de Ingresos").ToUpper(Cultura));
            AgregarTexto(sb, 35, 790, 8, false, GrisTexto,
                Tr("lblInformePeriodo", "Período:") + " " + periodo + "  |  " +
                Tr("lblLeyendaFacturacion", "Facturación (US$)") + "  |  " +
                Tr("lblLeyendaOperaciones", "Operaciones") + ": " + operaciones);

            DibujarGraficoBarras(sb, 48f, 460f, 499f, 290f,
                informe.SerieDiaria
                    .Select(d => new CategoriaVolumen { Etiqueta = d.Dia.ToString(Cultura), Monto = d.Monto })
                    .ToList(),
                false, 6.5f);

            AgregarTexto(sb, 35, 388, 12, true, AzulDarkline, Tr("lblGraficoVentasMarca", "Facturación por Marca / Modelo").ToUpper(Cultura));

            var marcas = informe.PorVehiculo.Take(6).ToList();
            DibujarBarrasHorizontales(sb, 45f, 110f, 505f, 260f, marcas, true);

            AgregarPiePagina(sb, totalPaginas, 2);
            return EncodingBytes(sb);
        }

        // ---------- Página 3: personalizados vs estándar + métodos de pago ----------

        private static byte[] GenerarPaginaGraficos2(DatosInformeVentas informe, int totalPaginas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("q");
            sb.AppendLine("1 1 1 rg 0 0 595.28 841.89 re f");

            string etiquetaPersonalizado = Tr("texto_personalizado_informe", "Personalizado");
            string etiquetaEstandar = Tr("texto_estandar_base", "Estándar de Fábrica");

            // Sección 1: Personalizados vs Estándar (barra segmentada con proporciones)
            AgregarTexto(sb, 35, 806, 12, true, AzulDarkline, Tr("lblGraficoPersonalizadosEstandar", "Personalizados vs. Estándar de Fábrica").ToUpper(Cultura));

            int totalUnidades = informe.CantidadPersonalizados + informe.CantidadEstandar;
            float barraX = 45f, barraY = 690f, barraAncho = 505f, barraAlto = 34f;
            float porcionPersonalizados = totalUnidades > 0
                ? barraAncho * (informe.CantidadPersonalizados / (float)totalUnidades)
                : barraAncho;

            sb.AppendLine(RojoRedline + " rg " + Num(barraX) + " " + Num(barraY) + " " + Num(porcionPersonalizados) + " " + Num(barraAlto) + " re f");

            float anchoEstandar = barraAncho - porcionPersonalizados;
            if (anchoEstandar > 0)
            {
                sb.AppendLine("0.35 0.39 0.47 rg " + Num(barraX + porcionPersonalizados) + " " + Num(barraY) + " " + Num(anchoEstandar) + " " + Num(barraAlto) + " re f");
            }

            decimal porcPersonalizado = totalUnidades > 0 ? 100m * informe.CantidadPersonalizados / totalUnidades : 0m;
            decimal porcEstandar = totalUnidades > 0 ? 100m * informe.CantidadEstandar / totalUnidades : 0m;

            AgregarTexto(sb, barraX + 8, barraY + 12, 9, true, "1 1 1",
                etiquetaPersonalizado + ": " + informe.CantidadPersonalizados + " (" + porcPersonalizado.ToString("0", Cultura) + "%) - " + Moneda(informe.MontoPersonalizados));
            AgregarTexto(sb, barraX + 8, barraY - 14, 8, false, GrisTexto,
                etiquetaEstandar + ": " + informe.CantidadEstandar + " (" + porcEstandar.ToString("0", Cultura) + "%) - " + Moneda(informe.MontoEstandar));

            // Sección 2: Métodos de pago
            AgregarTexto(sb, 35, 600, 12, true, AzulDarkline, Tr("lblGraficoMetodosPago", "Distribución por Método de Pago").ToUpper(Cultura));

            var pagos = informe.PorMetodoPago.Take(6).ToList();
            DibujarBarrasHorizontales(sb, 45f, 340f, 505f, 230f, pagos, true);

            AgregarPiePagina(sb, totalPaginas, 3);
            return EncodingBytes(sb);
        }

        // ---------- Páginas de detalle con paginación automática ----------

        private static byte[] GenerarPaginaDetalle(DatosInformeVentas informe, string periodo, int pagina, int filasPorPagina, int totalPaginas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("q");
            sb.AppendLine("1 1 1 rg 0 0 595.28 841.89 re f");

            AgregarTexto(sb, 35, 806, 12, true, AzulDarkline, Tr("pdfSeccionDetalleVentas", "Detalle de Ventas del Período").ToUpper(Cultura));
            AgregarTexto(sb, 35, 790, 8, false, GrisTexto, Tr("lblInformePeriodo", "Período:") + " " + periodo);

            var ventasOrdenadas = informe.VentasDelMes
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.NumeroVenta)
                .ToList();

            // Encabezado de la tabla
            float yTabla = 760f;
            sb.AppendLine(AzulDarkline + " rg 35 " + Num(yTabla) + " 525 18 re f");

            AgregarTexto(sb, 40, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("col_VentaNumero", "N° Venta"), 16));
            AgregarTexto(sb, 118, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("pdfColFactura", "N° Factura"), 18));
            AgregarTexto(sb, 205, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("col_VentaFecha", "Fecha"), 12));
            AgregarTexto(sb, 258, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("col_VentaCliente", "Cliente"), 14));
            AgregarTexto(sb, 336, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("col_VentaVehiculoBase", "Vehículo Base"), 18));
            AgregarTexto(sb, 444, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("pdfColMetodoPago", "Método Pago"), 11));
            AgregarTexto(sb, 508, yTabla + 5, 7.5f, true, "1 1 1", Truncar(Tr("col_VentaTotal", "Total"), 8));

            float y = yTabla - 18f;
            int indice = pagina * filasPorPagina;
            int fin = Math.Min(indice + filasPorPagina, ventasOrdenadas.Count);

            for (; indice < fin; indice++)
            {
                Venta v = ventasOrdenadas[indice];

                if ((indice % 2) == 0)
                {
                    sb.AppendLine("0.972 0.975 0.98 rg 35 " + Num(y - 3f) + " 525 16 re f");
                }
                sb.AppendLine("0.88 0.88 0.9 RG 0.4 w 35 " + Num(y - 3f) + " m 560 " + Num(y - 3f) + " l S");

                AgregarTexto(sb, 40, y + 1f, 7, false, "0.12 0.12 0.12", Truncar(string.IsNullOrEmpty(v.NumeroVenta) ? ("VTA-" + v.ID.ToString("D6", Cultura)) : v.NumeroVenta, 16));
                AgregarTexto(sb, 118, y + 1f, 7, false, "0.25 0.25 0.3", Truncar(v.Factura != null && !string.IsNullOrEmpty(v.Factura.NumeroFactura) ? v.Factura.NumeroFactura : "-", 18));
                AgregarTexto(sb, 205, y + 1f, 7, false, "0.3 0.3 0.35", v.Fecha.ToString("dd/MM/yyyy", Cultura));
                AgregarTexto(sb, 258, y + 1f, 7, false, "0.3 0.3 0.35", Truncar(ClienteDeVenta(v), 15));
                AgregarTexto(sb, 336, y + 1f, 7, false, "0.3 0.3 0.35", Truncar(EtiquetaVehiculo(v), 19));
                AgregarTexto(sb, 444, y + 1f, 7, false, "0.3 0.3 0.35", Truncar(MetodoPagoDeVenta(v), 12));
                AgregarTexto(sb, 512, y + 1f, 7, true, "0.12 0.12 0.12", Moneda(v.Total));

                y -= 16f;
            }

            AgregarPiePagina(sb, totalPaginas, 4 + pagina);
            return EncodingBytes(sb);
        }

        private static string ClienteDeVenta(Venta v)
        {
            var cliente = v != null ? v.Cliente : null;
            if (cliente == null ||
                (string.IsNullOrWhiteSpace(cliente.Nombre) && string.IsNullOrWhiteSpace(cliente.Apellido)))
            {
                return "-";
            }
            return (cliente.Nombre + " " + cliente.Apellido).Trim();
        }

        private static string Truncar(string texto, int max)
        {
            if (string.IsNullOrEmpty(texto)) return "-";
            texto = texto.Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")
                .Replace("ñ", "n").Replace("Ñ", "N");
            return texto.Length <= max ? texto : texto.Substring(0, max - 2) + "..";
        }

        // ---------- Primitivas de dibujo ----------

        private static void DibujarTarjetaKpi(StringBuilder sb, float x, float y, float ancho, float alto, string etiqueta, string valor)
        {
            sb.AppendLine(PanelClaro + " rg " + Num(x) + " " + Num(y) + " " + Num(ancho) + " " + Num(alto) + " re f");
            sb.AppendLine(BordePanel + " RG 1 w " + Num(x) + " " + Num(y) + " " + Num(ancho) + " " + Num(alto) + " re S");
            sb.AppendLine(RojoRedline + " rg " + Num(x) + " " + Num(y + alto - 4f) + " " + Num(ancho) + " 4 re f");

            AgregarTexto(sb, x + 10, y + alto - 24, 7, false, GrisTexto, Truncar(etiqueta, 26));
            AgregarTexto(sb, x + 10, y + alto - 42, 10.5f, true, AzulDarkline, Truncar(valor, 20));
        }

        private static void DibujarGraficoBarras(StringBuilder sb, float x, float y, float ancho, float alto,
            List<CategoriaVolumen> datos, bool mostrarValores, float tamanoEtiqueta)
        {
            if (datos == null || datos.Count == 0)
            {
                AgregarTexto(sb, x, y + alto / 2f, 9, false, GrisTexto, "-");
                return;
            }

            float margenInferior = 30f;
            float plotAlto = alto - margenInferior;
            float plotAncho = ancho;
            float baseline = y;

            decimal max = datos.Max(d => d.Monto);
            if (max <= 0) max = 1m;

            // Panel de fondo
            sb.AppendLine(PanelClaro + " rg " + Num(x - 10) + " " + Num(y - 20f) + " " + Num(ancho + 20) + " " + Num(alto + 36) + " re f");
            sb.AppendLine(BordePanel + " RG 1 w " + Num(x - 10) + " " + Num(y - 20f) + " " + Num(ancho + 20) + " " + Num(alto + 36) + " re S");

            // Grillas 50% / 100% con etiquetas del eje Y
            for (int g = 1; g <= 2; g++)
            {
                float gy = baseline + plotAlto * g / 2f;
                sb.AppendLine("0.82 0.83 0.86 RG 0.5 w " + Num(x) + " " + Num(gy) + " m " + Num(x + plotAncho) + " " + Num(gy) + " l S");
                AgregarTexto(sb, x - 40, gy - 3f, 6, false, GrisTexto, FormatoCorto(max * g / 2m));
            }
            sb.AppendLine("0.5 0.52 0.57 RG 1 w " + Num(x) + " " + Num(baseline) + " m " + Num(x + plotAncho) + " " + Num(baseline) + " l S");

            float slot = plotAncho / datos.Count;
            float anchoBarra = slot * 0.62f;
            if (anchoBarra > 22f) anchoBarra = 22f;

            int indice = 0;
            foreach (var dato in datos)
            {
                float altura = (float)(dato.Monto / max) * plotAlto;
                float bx = x + slot * indice + (slot - anchoBarra) / 2f;
                bool esMaximo = dato.Monto == max;

                string relleno = esMaximo ? RojoRedline : "0.35 0.39 0.47";

                sb.AppendLine(relleno + " rg " + Num(bx) + " " + Num(baseline + 1f) + " " + Num(anchoBarra) + " " + Num(Math.Max(altura, 0.8f)) + " re f");

                if (mostrarValores && datos.Count <= 12 && altura > 8f)
                {
                    AgregarTexto(sb, bx - 4f, baseline + altura + 6f, 6.5f, true, AzulDarkline, FormatoCorto(dato.Monto));
                }

                float centroEtiqueta = x + slot * indice + slot / 2f - tamanoEtiqueta;
                AgregarTexto(sb, centroEtiqueta, baseline - tamanoEtiqueta - 8f, tamanoEtiqueta, false, GrisTexto, dato.Etiqueta);
                indice++;
            }
        }

        private static void DibujarBarrasHorizontales(StringBuilder sb, float x, float y, float ancho, float alto,
            List<CategoriaVolumen> datos, bool mostrarMontos)
        {
            if (datos == null || datos.Count == 0)
            {
                sb.AppendLine(PanelClaro + " rg " + Num(x - 10) + " " + Num(y - 10f) + " " + Num(ancho + 20) + " " + Num(alto + 20) + " re f");
                sb.AppendLine(BordePanel + " RG 1 w " + Num(x - 10) + " " + Num(y - 10f) + " " + Num(ancho + 20) + " " + Num(alto + 20) + " re S");
                AgregarTexto(sb, x + 10, y + alto / 2f, 8.5f, false, GrisTexto, "-");
                return;
            }

            decimal max = datos.Max(d => d.Monto);
            if (max <= 0) max = 1m;
            decimal total = datos.Sum(d => d.Monto);

            float filaAlto = alto / datos.Count;
            float barraAlto = Math.Min(filaAlto * 0.5f, 20f);
            float barraX = x + 135f;
            float barraAnchoMax = ancho - 265f;

            sb.AppendLine(PanelClaro + " rg " + Num(x - 10) + " " + Num(y - 12f) + " " + Num(ancho + 20) + " " + Num(alto + 24) + " re f");
            sb.AppendLine(BordePanel + " RG 1 w " + Num(x - 10) + " " + Num(y - 12f) + " " + Num(ancho + 20) + " " + Num(alto + 24) + " re S");

            int indice = 0;
            foreach (var dato in datos)
            {
                float cy = y + alto - filaAlto * indice - filaAlto / 2f - barraAlto / 2f;
                decimal porcentaje = total > 0 ? 100m * dato.Monto / total : 0m;
                float largo = (float)(dato.Monto / max) * barraAnchoMax;

                AgregarTexto(sb, x, cy + barraAlto / 2f - 2f, 7.5f, false, GrisTexto, Truncar(dato.Etiqueta, 23));

                sb.AppendLine((indice == 0 ? RojoRedline : "0.35 0.39 0.47") + " rg " +
                               Num(barraX) + " " + Num(cy) + " " + Num(Math.Max(largo, 1.5f)) + " " + Num(barraAlto) + " re f");

                if (mostrarMontos)
                {
                    string colorValor = indice == 0 ? AzulDarkline : GrisTexto;
                    AgregarTexto(sb, barraX + barraAnchoMax + 10, cy + barraAlto / 2f - 2f, 7.5f, true, colorValor, Moneda(dato.Monto));
                    AgregarTexto(sb, barraX + barraAnchoMax + 84, cy + barraAlto / 2f - 2f, 7, false, colorValor, "(" + porcentaje.ToString("0.#", Cultura) + "%)");
                }

                indice++;
            }
        }

        private static void AgregarPiePagina(StringBuilder sb, int totalPaginas, int numeroPagina)
        {
            sb.AppendLine("q");
            sb.AppendLine(RojoRedline + " rg 35 45 525 2 re f");

            string etiquetaPagina = Tr("pdfTextoPagina", "Página") + " " +
                                     numeroPagina.ToString(Cultura) + " / " + totalPaginas.ToString(Cultura);
            AgregarTexto(sb, 35, 30, 7.5f, false, "0.5 0.5 0.55", Tr("pdfLeyendaPie", "Informe Mensual de Ventas generado automáticamente por RedLine Customs S.A."));
            AgregarTexto(sb, 496, 30, 7.5f, true, RojoRedline, etiquetaPagina);
            sb.AppendLine("Q");
        }

        // ---------- Helpers de texto y bytes ----------

        private static void AgregarTexto(StringBuilder sb, float x, float y, float tamano, bool negrita, string rgb, string texto)
        {
            sb.AppendLine("BT /F" + (negrita ? "2" : "1") + " " + tamano.ToString("0.##", Cultura) + " Tf " +
                           rgb + " rg " + Num(x) + " " + Num(y) + " Td (" + Escape(texto) + ") Tj ET");
        }

        private static string Num(float valor)
        {
            return valor.ToString("0.##", Cultura);
        }

        private static byte[] EncodingBytes(StringBuilder sb)
        {
            return Encoding.GetEncoding("ISO-8859-1").GetBytes(sb.ToString());
        }

        private static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            string normal = text
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")
                .Replace("ñ", "n").Replace("Ñ", "N")
                .Replace("°", "o").Replace("¿", "").Replace("¡", "")
                .Replace("€", "EUR ").Replace("–", "-").Replace("—", "-");
            return normal
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }

        private static byte[] ConstruirPdf(List<byte[]> streamsPorPagina)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms, Encoding.GetEncoding("ISO-8859-1")))
            {
                writer.NewLine = "\n";
                writer.WriteLine("%PDF-1.4");
                writer.WriteLine("%\xE2\xE3\xCF\xD3");
                writer.Flush();

                var offsets = new List<long>();
                int cantidad = streamsPorPagina.Count;

                // Numeración de objetos:
                //   1 Catalog / 2 Pages / 3 (F1 Helvetica) / 4 (F2 Helvetica-Bold)
                //   y por cada página k (base 0): Page = 5 + 2k, Contents = 6 + 2k

                offsets.Add(ms.Position);
                writer.WriteLine("1 0 obj");
                writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
                writer.WriteLine("endobj");
                writer.Flush();

                var kids = string.Join(" ", Enumerable.Range(0, cantidad).Select(k => (5 + 2 * k) + " 0 R"));
                offsets.Add(ms.Position);
                writer.WriteLine("2 0 obj");
                writer.WriteLine("<< /Type /Pages /Kids [" + kids + "] /Count " + cantidad + " >>");
                writer.WriteLine("endobj");
                writer.Flush();

                offsets.Add(ms.Position);
                writer.WriteLine("3 0 obj");
                writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");
                writer.WriteLine("endobj");
                writer.Flush();

                offsets.Add(ms.Position);
                writer.WriteLine("4 0 obj");
                writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>");
                writer.WriteLine("endobj");
                writer.Flush();

                for (int k = 0; k < cantidad; k++)
                {
                    byte[] streamBytes = streamsPorPagina[k];
                    int pageObj = 5 + 2 * k;
                    int contentsObj = 6 + 2 * k;

                    offsets.Add(ms.Position);
                    writer.WriteLine(pageObj + " 0 obj");
                    writer.WriteLine("<< /Type /Page /Parent 2 0 R");
                    writer.WriteLine("   /MediaBox [0 0 595.28 841.89]");
                    writer.WriteLine("   /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >>");
                    writer.WriteLine("   /Contents " + contentsObj + " 0 R");
                    writer.WriteLine(">>");
                    writer.WriteLine("endobj");
                    writer.Flush();

                    offsets.Add(ms.Position);
                    writer.WriteLine(contentsObj + " 0 obj");
                    writer.WriteLine("<< /Length " + streamBytes.Length + " >>");
                    writer.WriteLine("stream");
                    writer.Flush();

                    ms.Write(streamBytes, 0, streamBytes.Length);
                    ms.Flush();

                    writer.WriteLine();
                    writer.WriteLine("endstream");
                    writer.WriteLine("endobj");
                    writer.Flush();
                }

                long startXref = ms.Position;
                writer.WriteLine("xref");
                writer.WriteLine("0 " + (offsets.Count + 1));
                writer.WriteLine("0000000000 65535 f ");
                for (int i = 0; i < offsets.Count; i++)
                {
                    writer.WriteLine(offsets[i].ToString("D10") + " 00000 n ");
                }

                writer.WriteLine("trailer");
                writer.WriteLine("<< /Size " + (offsets.Count + 1) + " /Root 1 0 R >>");
                writer.WriteLine("startxref");
                writer.WriteLine(startXref);
                writer.WriteLine("%%EOF");
                writer.Flush();

                return ms.ToArray();
            }
        }
    }
}
