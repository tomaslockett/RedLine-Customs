using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be
{
    public abstract class Auto
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }

        public abstract double GetPrecio();
        public abstract string GetDescripcion();
        public abstract int GetVelocidad();
        public abstract int GetAceleracion();
        public bool TieneStock()
        {
            return Stock > 0;
        }
    }
}
