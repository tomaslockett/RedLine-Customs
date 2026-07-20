
using RedLine.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedLine.Be.Interfaces
{
    public interface IGestorIntegridad
    {
        void RecalcularIntegridad();
        ReporteIntegridad CalcularIntegridadActual();
        string ObtenerNombreTabla();
    }
}
