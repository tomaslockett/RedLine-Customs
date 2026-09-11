using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Entidades
{
    public class Idioma
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EsDefault { get; set; }
        public Dictionary<string, string> Traducciones { get; set; }

        public Idioma()
        {
            Traducciones = new Dictionary<string, string>();
        }

        public Idioma(int id, string nombre, bool esDefault)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.EsDefault = esDefault;
            this.Traducciones = new Dictionary<string, string>();
        }
    }
}
