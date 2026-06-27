using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Entidades
{
    public class Factura
    {
        public int ID { get; set; }
        public string NumeroFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }
        public int IVA { get; set; }
        public Venta Venta { get; set; }
        public Factura() { }
    }
}
