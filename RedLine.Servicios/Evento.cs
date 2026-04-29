using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios
{
    public class Evento
    {
        public int ID { get; set; } 
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Modulo { get; set; }
        public string Actividad { get; set; }
        public int Criticidad { get; set; } 

        public Evento() { }

        public Evento(string usuario, string modulo, string actividad, int criticidad)
        {
            this.Usuario = usuario;
            this.Fecha = DateTime.Now;
            this.Modulo = modulo;
            this.Actividad = actividad;
            this.Criticidad = criticidad;
        }
    }
}
