using RedLine.Be.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Entidades
{
    public class Etiqueta : IEntidad
    {
        public int ID { get; set; }
        public string Clave { get; set; }         
        public string Pagina { get; set; }         
        public string Descripcion { get; set; }    
        public bool EsMensajeAlerta { get; set; }  

        public Etiqueta() { }

        public Etiqueta(int id, string clave, string pagina, string descripcion, bool esMensajeAlerta = false)
        {
            ID = id;
            Clave = clave;
            Pagina = pagina;
            Descripcion = descripcion;
            EsMensajeAlerta = esMensajeAlerta;
        }
    }
}
