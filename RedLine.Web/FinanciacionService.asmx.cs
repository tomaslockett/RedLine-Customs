using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;

namespace RedLine.Web
{
    /// <summary>
    /// Servicio web ASMX para simulación de planes de financiación prendaria en USD (sistema francés),
    /// cotización de referencia en múltiples divisas y validación de tarjetas por algoritmo de Luhn.
    /// Funciona 100% de manera local y offline para REDLINE CUSTOMS S.A.
    /// </summary>
    [WebService(Namespace = "http://redlinecustoms.com/services")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class FinanciacionService : WebService
    {
        [WebMethod(Description = "Calcula un plan de amortización bajo sistema francés en USD para concesionaria de alta gama")]
        public PlanFinanciacionDto CalcularPlanFinanciacion(decimal montoUsd, int cuotas, decimal porcentajeAnticipo)
        {
            var resultado = new PlanFinanciacionDto();

            try
            {
                if (montoUsd < 0m) montoUsd = 0m;

                // Validación de cuotas permitidas (12, 24, 36 o 48). Por defecto 24
                if (cuotas != 12 && cuotas != 24 && cuotas != 36 && cuotas != 48)
                {
                    cuotas = 24;
                }

                // Validación de porcentaje de anticipo (10% a 80%). Por defecto 30%
                if (porcentajeAnticipo < 10m || porcentajeAnticipo > 80m)
                {
                    porcentajeAnticipo = 30m;
                }

                // Tasas TNA y CFT escalonadas según el plazo
                decimal tna = 0.115m;
                decimal cft = 0.135m;

                switch (cuotas)
                {
                    case 12:
                        tna = 0.0950m;
                        cft = 0.1120m;
                        break;
                    case 24:
                        tna = 0.1150m;
                        cft = 0.1350m;
                        break;
                    case 36:
                        tna = 0.1350m;
                        cft = 0.1580m;
                        break;
                    case 48:
                        tna = 0.1550m;
                        cft = 0.1810m;
                        break;
                }

                decimal anticipoUsd = Math.Round(montoUsd * (porcentajeAnticipo / 100m), 2);
                decimal capitalFinanciado = Math.Max(0m, montoUsd - anticipoUsd);
                decimal valorCuotaUsd = 0m;

                if (capitalFinanciado > 0m && cuotas > 0)
                {
                    decimal tasaMensual = tna / 12m;
                    double baseFactor = (double)(1m + tasaMensual);
                    double powFactor = Math.Pow(baseFactor, -cuotas);
                    double denominador = 1.0 - powFactor;

                    if (denominador > 0.0000001)
                    {
                        valorCuotaUsd = Math.Round(capitalFinanciado * (tasaMensual / (decimal)denominador), 2);
                    }
                    else
                    {
                        valorCuotaUsd = Math.Round(capitalFinanciado / cuotas, 2);
                    }
                }

                decimal totalPlanUsd = anticipoUsd + (valorCuotaUsd * cuotas);

                resultado.MontoVehiculoUsd = montoUsd;
                resultado.PorcentajeAnticipo = porcentajeAnticipo;
                resultado.AnticipoUsd = anticipoUsd;
                resultado.CapitalFinanciadoUsd = capitalFinanciado;
                resultado.Cuotas = cuotas;
                resultado.ValorCuotaUsd = valorCuotaUsd;
                resultado.TnaPorcentaje = Math.Round(tna * 100m, 2);
                resultado.CftPorcentaje = Math.Round(cft * 100m, 2);
                resultado.TotalPlanUsd = totalPlanUsd;
                resultado.ResumenPlan = $"{cuotas} cuotas de ${valorCuotaUsd:N2} USD (Anticipo: ${anticipoUsd:N2} USD)";
            }
            catch (Exception ex)
            {
                resultado.ResumenPlan = "Error en el cálculo: " + ex.Message;
            }

            return resultado;
        }

        [WebMethod(Description = "Convierte un monto USD a divisas de referencia comercial (ARS Oficial, ARS MEP, EUR, BRL)")]
        public CotizacionMonedaDto ConvertirMoneda(decimal montoUsd, string monedaDestino)
        {
            var resultado = new CotizacionMonedaDto();

            try
            {
                if (montoUsd < 0m) montoUsd = 0m;

                string cod = (monedaDestino ?? "USD").Trim().ToUpperInvariant();
                decimal tasa = 1.00m;
                string simbolo = "US$";
                string nombreCodigo = "USD";

                switch (cod)
                {
                    case "ARS_OFICIAL":
                    case "ARS":
                        tasa = 1050.00m;
                        simbolo = "$";
                        nombreCodigo = "ARS (Oficial)";
                        break;
                    case "ARS_MEP":
                    case "MEP":
                        tasa = 1220.00m;
                        simbolo = "$";
                        nombreCodigo = "ARS (MEP)";
                        break;
                    case "EUR":
                        tasa = 0.92m;
                        simbolo = "€";
                        nombreCodigo = "EUR";
                        break;
                    case "BRL":
                        tasa = 5.75m;
                        simbolo = "R$";
                        nombreCodigo = "BRL";
                        break;
                    case "USD":
                    default:
                        tasa = 1.00m;
                        simbolo = "US$";
                        nombreCodigo = "USD";
                        break;
                }

                decimal convertido = Math.Round(montoUsd * tasa, 2);

                resultado.MontoOriginalUsd = montoUsd;
                resultado.CodigoMoneda = nombreCodigo;
                resultado.Simbolo = simbolo;
                resultado.TasaCambio = tasa;
                resultado.MontoConvertido = convertido;
                resultado.MontoFormateado = $"{simbolo} {convertido:N2}";
                resultado.FechaReferencia = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                resultado.MontoOriginalUsd = montoUsd;
                resultado.CodigoMoneda = "USD";
                resultado.Simbolo = "US$";
                resultado.TasaCambio = 1.00m;
                resultado.MontoConvertido = montoUsd;
                resultado.MontoFormateado = $"US$ {montoUsd:N2}";
                resultado.FechaReferencia = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return resultado;
        }

        [WebMethod(Description = "Verifica validez de tarjeta mediante algoritmo de Luhn y detecta la marca comercial")]
        public ResultadoValidacionTarjetaDto ValidarTarjeta(string numeroTarjeta)
        {
            var resultado = new ResultadoValidacionTarjetaDto
            {
                EsValida = false,
                Marca = "Otra",
                UltimosCuatro = string.Empty,
                NumeroEnmascarado = string.Empty,
                Mensaje = string.Empty
            };

            try
            {
                string soloDigitos = new string((numeroTarjeta ?? string.Empty).Where(char.IsDigit).ToArray());

                if (soloDigitos.Length < 13 || soloDigitos.Length > 19)
                {
                    resultado.Mensaje = "Longitud de tarjeta inválida.";
                    return resultado;
                }

                // Detección de marca
                string marca = "Otra";
                bool longitudCorrectaPorMarca = true;

                if (soloDigitos.StartsWith("4"))
                {
                    marca = "Visa";
                    longitudCorrectaPorMarca = (soloDigitos.Length == 13 || soloDigitos.Length == 16);
                }
                else if (soloDigitos.StartsWith("34") || soloDigitos.StartsWith("37"))
                {
                    marca = "Amex";
                    longitudCorrectaPorMarca = (soloDigitos.Length == 15);
                }
                else
                {
                    // Mastercard: 51-55 o rango 2221-2720
                    int prefijo2 = soloDigitos.Length >= 2 && int.TryParse(soloDigitos.Substring(0, 2), out int p2) ? p2 : 0;
                    int prefijo4 = soloDigitos.Length >= 4 && int.TryParse(soloDigitos.Substring(0, 4), out int p4) ? p4 : 0;

                    if ((prefijo2 >= 51 && prefijo2 <= 55) || (prefijo4 >= 2221 && prefijo4 <= 2720))
                    {
                        marca = "Mastercard";
                        longitudCorrectaPorMarca = (soloDigitos.Length == 16);
                    }
                }

                resultado.Marca = marca;

                if (!longitudCorrectaPorMarca)
                {
                    resultado.Mensaje = "Longitud no coincide con la marca detectada.";
                    return resultado;
                }

                // Algoritmo de Luhn
                int suma = 0;
                bool alternar = false;

                for (int i = soloDigitos.Length - 1; i >= 0; i--)
                {
                    int n = soloDigitos[i] - '0';
                    if (alternar)
                    {
                        n *= 2;
                        if (n > 9) n -= 9;
                    }
                    suma += n;
                    alternar = !alternar;
                }

                bool luhnValido = (suma % 10 == 0);
                resultado.EsValida = luhnValido;

                string ultimos4 = soloDigitos.Length >= 4 ? soloDigitos.Substring(soloDigitos.Length - 4) : soloDigitos;
                resultado.UltimosCuatro = ultimos4;

                string mascara = new string('*', Math.Max(0, soloDigitos.Length - 4)) + ultimos4;
                resultado.NumeroEnmascarado = mascara;
                resultado.Mensaje = luhnValido ? "Tarjeta válida" : "Fallo en algoritmo de Luhn";
            }
            catch (Exception ex)
            {
                resultado.EsValida = false;
                resultado.Mensaje = "Error de validación: " + ex.Message;
            }

            return resultado;
        }
    }

    public class PlanFinanciacionDto
    {
        public decimal MontoVehiculoUsd { get; set; }
        public decimal PorcentajeAnticipo { get; set; }
        public decimal AnticipoUsd { get; set; }
        public decimal CapitalFinanciadoUsd { get; set; }
        public int Cuotas { get; set; }
        public decimal ValorCuotaUsd { get; set; }
        public decimal TnaPorcentaje { get; set; }
        public decimal CftPorcentaje { get; set; }
        public decimal TotalPlanUsd { get; set; }
        public string ResumenPlan { get; set; }
    }

    public class CotizacionMonedaDto
    {
        public decimal MontoOriginalUsd { get; set; }
        public string CodigoMoneda { get; set; }
        public string Simbolo { get; set; }
        public decimal TasaCambio { get; set; }
        public decimal MontoConvertido { get; set; }
        public string MontoFormateado { get; set; }
        public string FechaReferencia { get; set; }
    }

    public class ResultadoValidacionTarjetaDto
    {
        public bool EsValida { get; set; }
        public string Marca { get; set; }
        public string UltimosCuatro { get; set; }
        public string NumeroEnmascarado { get; set; }
        public string Mensaje { get; set; }
    }
}
