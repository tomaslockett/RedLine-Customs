using System;

namespace Redline.Be
{
    public class Pintura : Mejora
    {
        public string TipoAcabado { get; set; } // Mate, Satinado, Brillante, Cromado
        public string CodigoColor { get; set; } // Hexadecimal o código de marca
        public bool EsVinilo { get; set; } // True si es Wrap, False si es pintura real

        public Pintura() { }
    }
}