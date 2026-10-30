using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios
{
    public class PdfFacturaServicio
    {
        public static byte[] GenerarPdfFactura(Factura factura)
        {
            if (factura == null) throw new ArgumentNullException(nameof(factura));

            var venta = factura.Venta;
            var cliente = venta?.Cliente;
            var autoBase = venta?.AutoBase ?? venta?.AutoPersonalizado?.AuBase;
            var autoPers = venta?.AutoPersonalizado;

            var sbContent = new StringBuilder();

            // Fondo blanco para la página completa
            sbContent.AppendLine("q");
            sbContent.AppendLine("1 1 1 rg 0 0 595.28 841.89 re f");

            // Header Banner (Rojo RedLine)
            sbContent.AppendLine("0.812 0.165 0.114 rg 0 760 595.28 82 re f");

            // Línea de acento inferior en banner
            sbContent.AppendLine("0.1 0.1 0.1 RG 2 w 0 760 m 595.28 760 l S");

            // Título de la empresa en banner
            sbContent.AppendLine("BT /F2 22 Tf 1 1 1 rg 35 798 Td (" + Escape("REDLINE CUSTOMS") + ") Tj ET");
            sbContent.AppendLine("BT /F1 9 Tf 0.95 0.95 0.95 rg 35 782 Td (" + Escape("CONCESIONARIA Y TALLER DE PERSONALIZACION DE ALTA GAMA") + ") Tj ET");

            // Título FACTURA en banner derecho
            sbContent.AppendLine("BT /F2 16 Tf 1 1 1 rg 410 802 Td (" + Escape("FACTURA DIGITAL") + ") Tj ET");
            sbContent.AppendLine("BT /F1 9 Tf 1 1 1 rg 410 786 Td (" + Escape("ORIGINAL - COMPROBANTE OFICIAL") + ") Tj ET");
            sbContent.AppendLine("Q");

            // Recuadro Tipo Comprobante (Letra "B" / "A" en el centro superior)
            sbContent.AppendLine("q");
            sbContent.AppendLine("1 1 1 rg 275 745 45 42 re f");
            sbContent.AppendLine("0.812 0.165 0.114 RG 2 w 275 745 45 42 re S");
            sbContent.AppendLine("BT /F2 20 Tf 0.812 0.165 0.114 rg 289 762 Td (" + Escape("B") + ") Tj ET");
            sbContent.AppendLine("BT /F2 7 Tf 0.2 0.2 0.2 rg 284 750 Td (" + Escape("COD. 06") + ") Tj ET");
            sbContent.AppendLine("Q");

            // Recuadro de Datos Fiscales del Emisor y de la Factura
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.97 0.97 0.98 rg 35 660 525 80 re f");
            sbContent.AppendLine("0.8 0.8 0.85 RG 1 w 35 660 525 80 re S");
            sbContent.AppendLine("0.8 0.8 0.85 RG 1 w 297.64 660 m 297.64 740 l S");

            // Datos Emisor (Columna Izquierda)
            sbContent.AppendLine("BT /F2 9 Tf 0.15 0.15 0.15 rg 45 723 Td (" + Escape("REDLINE CUSTOMS S.A.") + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 45 710 Td (" + Escape("CUIT: 30-71829384-9 | IVA Responsable Inscripto") + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 45 697 Td (" + Escape("Ingresos Brutos: 30-71829384-9") + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 45 684 Td (" + Escape("Domicilio: Av. del Libertador 4500, CABA") + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 45 671 Td (" + Escape("Inicio de Actividades: 01/03/2020") + ") Tj ET");

            // Datos Comprobante (Columna Derecha)
            string numFactura = factura.NumeroFactura ?? ("FC-0001-" + factura.ID.ToString("D8"));
            string fechaEmision = factura.FechaEmision != DateTime.MinValue ? factura.FechaEmision.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string numVenta = venta?.NumeroVenta ?? ("VTA-" + factura.ID.ToString("D6"));
            string metodoPago = factura.MetodoPago ?? venta?.MetodoPago ?? "Tarjeta de Credito";

            sbContent.AppendLine("BT /F2 10 Tf 0.812 0.165 0.114 rg 310 723 Td (" + Escape("FACTURA Nro: " + numFactura) + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 310 710 Td (" + Escape("Fecha de Emision: " + fechaEmision) + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 310 697 Td (" + Escape("Numero de Venta: " + numVenta) + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.3 0.3 0.3 rg 310 684 Td (" + Escape("Condicion de Venta: Contado Inmediato") + ") Tj ET");
            sbContent.AppendLine("BT /F2 8 Tf 0.1 0.5 0.2 rg 310 671 Td (" + Escape("Metodo de Pago: " + metodoPago) + ") Tj ET");
            sbContent.AppendLine("Q");

            // Recuadro de Datos del Cliente
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.94 0.95 0.97 rg 35 595 525 55 re f");
            sbContent.AppendLine("0.8 0.8 0.85 RG 1 w 35 595 525 55 re S");

            string nomCliente = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}".Trim() : "Consumidor Final";
            string dniCliente = cliente != null && !string.IsNullOrEmpty(cliente.DNI) ? cliente.DNI : "Consumidor Final";
            string emailCliente = cliente != null && !string.IsNullOrEmpty(cliente.Email) ? cliente.Email : "-";
            string telCliente = cliente != null && !string.IsNullOrEmpty(cliente.Telefono) ? cliente.Telefono : "-";

            sbContent.AppendLine("BT /F2 9 Tf 0.15 0.15 0.15 rg 45 635 Td (" + Escape("DATOS DEL CLIENTE / RECEPTOR") + ") Tj ET");
            sbContent.AppendLine("BT /F1 8.5 Tf 0.25 0.25 0.25 rg 45 620 Td (" + Escape("Senor/a: " + nomCliente) + ") Tj ET");
            sbContent.AppendLine("BT /F1 8.5 Tf 0.25 0.25 0.25 rg 45 605 Td (" + Escape("DNI / CUIT: " + dniCliente) + ") Tj ET");

            sbContent.AppendLine("BT /F1 8.5 Tf 0.25 0.25 0.25 rg 310 620 Td (" + Escape("Email: " + emailCliente) + ") Tj ET");
            sbContent.AppendLine("BT /F1 8.5 Tf 0.25 0.25 0.25 rg 310 605 Td (" + Escape("Telefono: " + telCliente) + ") Tj ET");
            sbContent.AppendLine("Q");

            // Tabla de Conceptos Facturados
            // Encabezado de la tabla
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.15 0.17 0.22 rg 35 565 525 20 re f");
            sbContent.AppendLine("BT /F2 8.5 Tf 1 1 1 rg 45 572 Td (" + Escape("DESCRIPCION / CONCEPTO") + ") Tj ET");
            sbContent.AppendLine("BT /F2 8.5 Tf 1 1 1 rg 340 572 Td (" + Escape("CANT") + ") Tj ET");
            sbContent.AppendLine("BT /F2 8.5 Tf 1 1 1 rg 400 572 Td (" + Escape("PRECIO UNIT.") + ") Tj ET");
            sbContent.AppendLine("BT /F2 8.5 Tf 1 1 1 rg 490 572 Td (" + Escape("SUBTOTAL") + ") Tj ET");
            sbContent.AppendLine("Q");

            // Items de la factura
            float currentY = 542f;
            int itemIndex = 0;

            // 1. Vehículo Base
            if (autoBase != null)
            {
                string descAuto = $"Vehiculo: {autoBase.Marca} {autoBase.Modelo} ({autoBase.Anio}) - Motor {autoBase.Potencia} HP";
                string precioAuto = "$" + autoBase.PrecioBase.ToString("N0", CultureInfo.InvariantCulture);

                DrawTableRow(sbContent, currentY, itemIndex % 2 == 0, descAuto, "1", precioAuto, precioAuto);
                currentY -= 22f;
                itemIndex++;
            }

            // 2. Personalizaciones / Extras
            if (autoPers != null && autoPers.Mejoras != null && autoPers.Mejoras.Count > 0)
            {
                foreach (var extra in autoPers.Mejoras)
                {
                    string descExtra = $"Personalizacion: {extra.Nombre} ({extra.Categoria})";
                    string precioExtra = "$" + extra.Precio.ToString("N0", CultureInfo.InvariantCulture);

                    DrawTableRow(sbContent, currentY, itemIndex % 2 == 0, descExtra, "1", precioExtra, precioExtra);
                    currentY -= 22f;
                    itemIndex++;
                }
            }
            else if (!string.IsNullOrEmpty(autoPers?.NombreExtra))
            {
                string[] parts = autoPers.NombreExtra.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    string descExtra = "Personalizacion: " + p.Trim();
                    DrawTableRow(sbContent, currentY, itemIndex % 2 == 0, descExtra, "1", "Incluido", "Incluido");
                    currentY -= 22f;
                    itemIndex++;
                }
            }

            // Línea separadora de tabla
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.8 0.8 0.85 RG 1 w 35 " + currentY.ToString("0.00", CultureInfo.InvariantCulture) + " m 560 " + currentY.ToString("0.00", CultureInfo.InvariantCulture) + " l S");
            sbContent.AppendLine("Q");

            // Recuadro de Totales (a la derecha)
            float totalBoxY = currentY - 75f;
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.97 0.97 0.98 rg 330 " + totalBoxY.ToString("0.00", CultureInfo.InvariantCulture) + " 230 65 re f");
            sbContent.AppendLine("0.8 0.8 0.85 RG 1 w 330 " + totalBoxY.ToString("0.00", CultureInfo.InvariantCulture) + " 230 65 re S");

            decimal subtotal = factura.Subtotal > 0 ? factura.Subtotal : (factura.Total / 1.21m);
            decimal iva = factura.MontoIVA > 0 ? factura.MontoIVA : (factura.Total - subtotal);
            decimal total = factura.Total > 0 ? factura.Total : (subtotal + iva);

            string txtSubtotal = "$" + subtotal.ToString("N0", CultureInfo.InvariantCulture);
            string txtIva = "$" + iva.ToString("N0", CultureInfo.InvariantCulture);
            string txtTotal = "$" + total.ToString("N0", CultureInfo.InvariantCulture);

            float line1Y = totalBoxY + 48f;
            float line2Y = totalBoxY + 32f;
            float line3Y = totalBoxY + 12f;

            sbContent.AppendLine("BT /F1 9 Tf 0.3 0.3 0.3 rg 345 " + line1Y.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape("Subtotal Neto:") + ") Tj ET");
            sbContent.AppendLine("BT /F2 9 Tf 0.1 0.1 0.1 rg 480 " + line1Y.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(txtSubtotal) + ") Tj ET");

            sbContent.AppendLine("BT /F1 9 Tf 0.3 0.3 0.3 rg 345 " + line2Y.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape("IVA (21%):") + ") Tj ET");
            sbContent.AppendLine("BT /F2 9 Tf 0.1 0.1 0.1 rg 480 " + line2Y.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(txtIva) + ") Tj ET");

            // Total con fondo de acento
            sbContent.AppendLine("0.812 0.165 0.114 rg 330 " + totalBoxY.ToString("0.00", CultureInfo.InvariantCulture) + " 230 24 re f");
            sbContent.AppendLine("BT /F2 11 Tf 1 1 1 rg 345 " + line3Y.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape("TOTAL GENERAL:") + ") Tj ET");
            sbContent.AppendLine("BT /F2 12 Tf 1 1 1 rg 475 " + line3Y.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(txtTotal) + ") Tj ET");
            sbContent.AppendLine("Q");

            // Recuadro de Seguridad / CAE Fiscal simulado
            float caeY = totalBoxY - 80f;
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.96 0.96 0.97 rg 35 " + caeY.ToString("0.00", CultureInfo.InvariantCulture) + " 525 60 re f");
            sbContent.AppendLine("0.8 0.8 0.85 RG 1 w 35 " + caeY.ToString("0.00", CultureInfo.InvariantCulture) + " 525 60 re S");

            sbContent.AppendLine("BT /F2 8 Tf 0.2 0.2 0.2 rg 45 " + (caeY + 45f).ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape("COMPROBANTE AUTORIZADO ELECTRONICAMENTE") + ") Tj ET");
            sbContent.AppendLine("BT /F1 8 Tf 0.35 0.35 0.35 rg 45 " + (caeY + 30f).ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape("CAE Nro: 74928301928374 | Fecha de Vencimiento de CAE: " + DateTime.Now.AddDays(10).ToString("dd/MM/yyyy")) + ") Tj ET");
            sbContent.AppendLine("BT /F1 7.5 Tf 0.45 0.45 0.45 rg 45 " + (caeY + 15f).ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape("Comprobante generado conforme a las normas de facturacion electronica vigentes de RedLine Customs.") + ") Tj ET");
            sbContent.AppendLine("Q");

            // Pie de Página
            sbContent.AppendLine("q");
            sbContent.AppendLine("0.812 0.165 0.114 rg 35 45 525 2 re f");
            sbContent.AppendLine("BT /F1 8 Tf 0.5 0.5 0.5 rg 35 30 Td (" + Escape("RedLine Customs Argentina - Garantia oficial de taller 3 anos o 100.000 km en modificaciones certificadas.") + ") Tj ET");
            sbContent.AppendLine("BT /F2 8 Tf 0.812 0.165 0.114 rg 455 30 Td (" + Escape("www.redlinecustoms.com") + ") Tj ET");
            sbContent.AppendLine("Q");

            string streamBody = sbContent.ToString();
            byte[] streamBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(streamBody);

            return ConstruirPdf(streamBytes);
        }

        private static void DrawTableRow(StringBuilder sb, float y, bool zebra, string desc, string cant, string precio, string subtotal)
        {
            sb.AppendLine("q");
            if (zebra)
            {
                sb.AppendLine("0.98 0.98 0.99 rg 35 " + y.ToString("0.00", CultureInfo.InvariantCulture) + " 525 20 re f");
            }
            sb.AppendLine("0.9 0.9 0.92 RG 0.5 w 35 " + y.ToString("0.00", CultureInfo.InvariantCulture) + " m 560 " + y.ToString("0.00", CultureInfo.InvariantCulture) + " l S");

            float textY = y + 5f;
            sb.AppendLine("BT /F1 8 Tf 0.15 0.15 0.15 rg 45 " + textY.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(desc) + ") Tj ET");
            sb.AppendLine("BT /F1 8 Tf 0.25 0.25 0.25 rg 345 " + textY.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(cant) + ") Tj ET");
            sb.AppendLine("BT /F1 8 Tf 0.25 0.25 0.25 rg 405 " + textY.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(precio) + ") Tj ET");
            sb.AppendLine("BT /F2 8 Tf 0.15 0.15 0.15 rg 495 " + textY.ToString("0.00", CultureInfo.InvariantCulture) + " Td (" + Escape(subtotal) + ") Tj ET");
            sb.AppendLine("Q");
        }

        private static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            // Quitar o normalizar tildes para fuente estándar Type1
            string normal = text
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")
                .Replace("ñ", "n").Replace("Ñ", "N")
                .Replace("°", "o").Replace("¿", "").Replace("¡", "");

            return normal
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }

        private static byte[] ConstruirPdf(byte[] streamBytes)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms, Encoding.GetEncoding("ISO-8859-1")))
            {
                writer.NewLine = "\n";

                // Encabezado
                writer.WriteLine("%PDF-1.4");
                writer.WriteLine("%\xE2\xE3\xCF\xD3");
                writer.Flush();

                var offsets = new List<long>();

                // Obj 1: Catalog
                offsets.Add(ms.Position);
                writer.WriteLine("1 0 obj");
                writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
                writer.WriteLine("endobj");
                writer.Flush();

                // Obj 2: Pages
                offsets.Add(ms.Position);
                writer.WriteLine("2 0 obj");
                writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
                writer.WriteLine("endobj");
                writer.Flush();

                // Obj 3: Page (A4: 595.28 x 841.89)
                offsets.Add(ms.Position);
                writer.WriteLine("3 0 obj");
                writer.WriteLine("<< /Type /Page /Parent 2 0 R");
                writer.WriteLine("   /MediaBox [0 0 595.28 841.89]");
                writer.WriteLine("   /Resources <<");
                writer.WriteLine("      /Font <<");
                writer.WriteLine("         /F1 4 0 R");
                writer.WriteLine("         /F2 5 0 R");
                writer.WriteLine("      >>");
                writer.WriteLine("   >>");
                writer.WriteLine("   /Contents 6 0 R");
                writer.WriteLine(">>");
                writer.WriteLine("endobj");
                writer.Flush();

                // Obj 4: Font Helvetica
                offsets.Add(ms.Position);
                writer.WriteLine("4 0 obj");
                writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>");
                writer.WriteLine("endobj");
                writer.Flush();

                // Obj 5: Font Helvetica-Bold
                offsets.Add(ms.Position);
                writer.WriteLine("5 0 obj");
                writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>");
                writer.WriteLine("endobj");
                writer.Flush();

                // Obj 6: Contents stream
                offsets.Add(ms.Position);
                writer.WriteLine("6 0 obj");
                writer.WriteLine("<< /Length " + streamBytes.Length + " >>");
                writer.WriteLine("stream");
                writer.Flush();

                ms.Write(streamBytes, 0, streamBytes.Length);
                ms.Flush();

                writer.WriteLine();
                writer.WriteLine("endstream");
                writer.WriteLine("endobj");
                writer.Flush();

                // Cross-Reference Table
                long startXref = ms.Position;
                writer.WriteLine("xref");
                writer.WriteLine("0 " + (offsets.Count + 1));
                writer.WriteLine("0000000000 65535 f ");
                for (int i = 0; i < offsets.Count; i++)
                {
                    writer.WriteLine(offsets[i].ToString("D10") + " 00000 n ");
                }

                // Trailer
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
