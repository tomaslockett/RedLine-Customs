using System; 
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace RedLine.Web
{
    /// <summary>
    /// Descripción breve de CalcularImpuestos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class CalcularImpuestos : System.Web.Services.WebService
    {

        [WebMethod]
        public decimal CalcularImpuesto(decimal valorAutoSINIVA)
        {
            decimal iva = Math.Round(valorAutoSINIVA * 0.21m, 2);



            decimal precioTotal =  iva;

            return precioTotal;
        }
    }
}
