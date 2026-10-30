using RedLine.Be.Interfaces;
using System;

namespace Redline.Be
{
    public class Mejora : IEntidad
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; } // Ej: "Aerodinámica", "Suspensión", "Ruedas"

        // Opcional para guardar detalles q antes tenian las clases hijas
        public string Detalle { get; set; }

        public Mejora() { }
    }
}