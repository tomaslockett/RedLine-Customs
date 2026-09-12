using RedLine.Be.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Entidades
{
    public class Traduccion : IEntidad
    {
        public int ID { get; set; }
        public int IdIdioma { get; set; }
        public Idioma Idioma { get; set; }
        public int IdEtiqueta { get; set; }
        public Etiqueta Etiqueta { get; set; }
        public string Texto { get; set; }

        public Traduccion() { }

        public Traduccion(int id, int idIdioma, int idEtiqueta, string texto)
        {
            ID = id;
            IdIdioma = idIdioma;
            IdEtiqueta = idEtiqueta;
            Texto = texto;
        }
    }
}
