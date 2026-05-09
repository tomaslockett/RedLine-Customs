using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RedLine.Be.Entidades
{
    public class DigitoVerificador
    {
        [Key]
        public string NombreTabla { get; set; }
        public string DVH { get; set; }
        public string DVV { get; set; }
        
    }
}
