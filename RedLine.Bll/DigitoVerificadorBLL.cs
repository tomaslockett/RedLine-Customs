using RedLine.Be.Entidades;
using RedLine.Be.Interfaces;
using RedLine.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class DigitoVerificadorBLL : AbstractBLL<string, DigitoVerificador>
    {
        public DigitoVerificadorBLL() : base(new DAL_DigitoVerificador())
        {
        }

        public void RecalcularTodaLaBaseDeDatos()
        {
            var listaBLLs = ObtenerBLLsVerificables();

            foreach (var bll in listaBLLs)
            {
                bll.RecalcularIntegridad();
            }
        }

        public string VerificarTodaLaBaseDeDatos()
        {
            var reporteErrores = new StringBuilder();

            List<DigitoVerificador> dvGuardados = this.Listar();

            var listaBLLs = ObtenerBLLsVerificables();

            foreach (var bll in listaBLLs)
            {
                string nombreTabla = bll.ObtenerNombreTabla();
                var (dvhActual, dvvActual) = bll.CalcularIntegridadActual();

                if (dvhActual == "N/A") continue;

                var dvGuardado = dvGuardados.FirstOrDefault(dv => dv.NombreTabla == nombreTabla);

                if (dvGuardado == null)
                {
                    reporteErrores.AppendLine($"ERROR CRÍTICO: La tabla '{nombreTabla}' no tiene DV guardados en el sistema.");
                }
                else if (dvGuardado.DVH != dvhActual || dvGuardado.DVV != dvvActual)
                {
                    reporteErrores.AppendLine($"CORRUPCIÓN DETECTADA: La tabla '{nombreTabla}' ha sido modificada externamente.");
                }
            }

            if (reporteErrores.Length == 0)
            {
                return "OK. La integridad de la base de datos es 100% correcta.";
            }

            return reporteErrores.ToString();
        }

        private List<IGestorIntegridad> ObtenerBLLsVerificables()
        {
            var listaInstancias = new List<IGestorIntegridad>();

            var tiposVerificables = Assembly.GetExecutingAssembly().GetTypes().Where(tipo => typeof(IGestorIntegridad).IsAssignableFrom(tipo) && !tipo.IsInterface && !tipo.IsAbstract && tipo != typeof(DigitoVerificadorBLL)); 

            foreach (var tipo in tiposVerificables)
            {
                var instancia = (IGestorIntegridad)Activator.CreateInstance(tipo);
                listaInstancias.Add(instancia);
            }

            return listaInstancias;
        }

    }
}
