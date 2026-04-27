using System;

namespace Redline.Be
{
    public abstract class Mejora
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; } 
        public string Categoria { get; set; } 

        public Mejora() { }
    }
}