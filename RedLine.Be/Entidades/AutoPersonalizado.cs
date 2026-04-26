using System;
using System.Collections.Generic;
using System.Linq;

namespace Redline.Be
{
    public class AutoPersonalizado
    {
        public int ID { get; set; }
        public string DNI_Cliente { get; set; }
        public AutoBase AuBase { get; set; }
        public List<Mejora> Mejoras { get; set; } = new List<Mejora>();
        public decimal PrecioFinal
        {
            get
            {
                decimal totalMejoras = Mejoras.Sum(m => m.Precio);
                return AuBase != null ? AuBase.PrecioBase + totalMejoras : totalMejoras;
            }
        }

        public AutoPersonalizado() { }
    }
}