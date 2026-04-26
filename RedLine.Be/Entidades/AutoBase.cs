using System;

namespace Redline.Be
{
    public class AutoBase
    {
        public int ID { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public decimal PrecioBase { get; set; }

        public AutoBase() { }

        public AutoBase(int id, string marca, string modelo, decimal precioBase)
        {
            this.ID = id;
            this.Marca = marca;
            this.Modelo = modelo;
            this.PrecioBase = precioBase;
        }
    }
}