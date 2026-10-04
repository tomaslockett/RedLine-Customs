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
        public Venta() { }

    }
}
