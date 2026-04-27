using System;

namespace Redline.Be
{
    public class Suspension : Mejora
    {
        public string Tipo { get; set; } // Neumática, Coilovers, Espirales Progresivos
        public double ReduccionAltura { get; set; } // En milímetros
        public int NivelesDureza { get; set; } // Cantidad de posiciones de ajuste

        public Suspension() { }
    }
}