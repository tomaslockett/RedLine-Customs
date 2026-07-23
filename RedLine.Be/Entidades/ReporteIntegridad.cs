using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Entidades
{
    public class ReporteIntegridad
    {
        public string DVH_Actual { get; set; } = "0";
        public string DVV_Actual { get; set; } = "0";
        public List<string> FilasCorruptasIDs { get; set; } = new List<string>();
        public string NombreColumnaID { get; set; } = "ID";
        public bool EsValido => FilasCorruptasIDs.Count == 0;
    }
}
