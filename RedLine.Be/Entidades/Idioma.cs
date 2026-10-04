using RedLine.Be.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Entidades
{
    public class Idioma : IEntidad
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public bool EsDefault { get; set; }

        public List<Traduccion> ListaTraducciones { get; set; }
        public Dictionary<string, string> Traducciones { get; set; }

        public Idioma()
        {
            ListaTraducciones = new List<Traduccion>();
            Traducciones = new Dictionary<string, string>();
        }

        public Idioma(int id, string nombre, bool esDefault)
        {
            this.ID = id;
            this.Nombre = nombre;
            this.EsDefault = esDefault;
            this.ListaTraducciones = new List<Traduccion>();
            this.Traducciones = new Dictionary<string, string>();
        }
    }
}
