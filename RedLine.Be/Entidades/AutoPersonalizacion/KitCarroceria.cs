using System;

namespace Redline.Be
{
    public class KitCarroceria : Mejora
    {
        public string Material { get; set; } // Fibra de Carbono, Plástico ABS, Vidrio
        public int CantidadPiezas { get; set; } // Cuántas partes incluye el kit
        public bool RequierePintura { get; set; } // Si viene en primer o listo para usar

        public KitCarroceria() { }
    }
}