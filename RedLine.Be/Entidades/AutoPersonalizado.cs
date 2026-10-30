using RedLine.Be.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redline.Be
{
    public class AutoPersonalizado : IEntidad
    {
        public int ID { get; set; }
        public string DNI_Cliente { get; set; }
        public AutoBase AuBase { get; set; }
        public List<Mejora> Mejoras { get; set; } = new List<Mejora>();

        private string _nombreExtra;
        public string NombreExtra
        {
            get
            {
                if (!string.IsNullOrEmpty(_nombreExtra)) return _nombreExtra;
                if (Mejoras != null && Mejoras.Count > 0)
                    return string.Join(", ", Mejoras.Select(m => m.Nombre));
                return null;
            }
            set { _nombreExtra = value; }
        }

        public DateTime? FechaCompra { get; set; }
        public string NumeroVenta { get; set; }
        public string NumeroFactura { get; set; }
        public string MetodoPago { get; set; }
        public decimal TotalPagado { get; set; }

        public decimal PrecioFinal
        {
            get
            {
                decimal totalMejoras = Mejoras != null ? Mejoras.Sum(m => m.Precio) : 0;
                return AuBase != null ? AuBase.PrecioBase + totalMejoras : totalMejoras;
            }
        }

        // Propiedades seguras para DataBinding y presentación desacoplada
        public string Marca => AuBase?.Marca ?? "RedLine";
        public string Modelo => AuBase?.Modelo ?? "Custom Edition";
        public int Anio => AuBase != null && AuBase.Anio > 0 ? AuBase.Anio : DateTime.Now.Year;
        public int VelocidadMaxima => AuBase != null && AuBase.VelocidadMaxima > 0 ? AuBase.VelocidadMaxima : 260;
        public int Potencia => AuBase != null && AuBase.Potencia > 0 ? AuBase.Potencia : 300;
        public decimal Aceleracion0a100 => AuBase != null && AuBase.Aceleracion0a100 > 0 ? AuBase.Aceleracion0a100 : 3.8m;
        public decimal PrecioBase => AuBase != null ? AuBase.PrecioBase : 0m;

        public AutoPersonalizado() { }
    }
}
