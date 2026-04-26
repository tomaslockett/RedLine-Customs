using System;

namespace Redline.Be
{
    public class Aleron : Mejora
    {
        public string Estilo { get; set; } // GT Wing, Ducktail, Tipo Original
        public bool EsAjustable { get; set; } // Si se puede cambiar el ángulo
        public double CargaAerodinamica { get; set; } // Valor simulado de presión

        public Aleron() { }
    }
}
