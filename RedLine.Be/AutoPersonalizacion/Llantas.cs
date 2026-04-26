using System;

namespace Redline.Be
{
    public class Llantas : Mejora
    {
        public int Rodado { get; set; } // 17, 18, 19, 20 pulgadas
        public double Ancho { get; set; } // Ancho en pulgadas (ej: 8.5, 9.5)
        public string Terminacion { get; set; } // Pulido, Mate, Diamantado

        public Llantas() { }
    }
}