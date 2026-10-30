using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redline.Be;
using RedLine.Be.Interfaces;

namespace RedLine.Be.Entidades 
{
    public class Venta : IEntidad
    {
        public int ID { get; set; }
        public string NumeroVenta { get; set; }
        public Cliente Cliente { get; set; }
        public AutoBase AutoBase { get; set; }
        public AutoPersonalizado AutoPersonalizado { get; set; }
        public DateTime Fecha { get; set; }
        public int IVA { get; set; }
        public decimal Total { get; set; }
        public Factura Factura { get; set; }
        public string MetodoPago { get; set; }

        public decimal Subtotal
        {
            get
            {
                if (AutoPersonalizado != null && AutoPersonalizado.PrecioFinal > 0)
                    return AutoPersonalizado.PrecioFinal;
                if (AutoBase != null && AutoBase.PrecioBase > 0)
                    return AutoBase.PrecioBase;
                if (Total > 0 && IVA > 0)
                    return Math.Round(Total / (1 + (IVA / 100m)), 2);
                return Total;
            }
        }

        public Venta() { }
    }
}
