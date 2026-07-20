using RedLine.Be.Entidades;
using RedLine.Be.Interfaces;
using RedLine.Dal;
using RedLine.Entidades;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_DigitoVerificador : AbstractBLL<string, DigitoVerificador>
    {
        private BLL_Evento _bllEvento;
        public BLL_DigitoVerificador() : base(new DAL_DigitoVerificador())
        {
            _bllEvento = new BLL_Evento();
        }

        public void RecalcularUnaTabla(string nombreTabla)
        {
            var bll = ObtenerBLLsVerificables().FirstOrDefault(b => b.ObtenerNombreTabla().Equals(nombreTabla, StringComparison.OrdinalIgnoreCase));

            if (bll == null) throw new Exception($"La tabla '{nombreTabla}' no existe o no tiene Digitos Verificadores.");

            bll.RecalcularIntegridad();
            RegistrarEventoBitacora($"Se forzó el recálculo de los dígitos verificadores SOLO para la tabla: {nombreTabla}", 2);
        }

        public void RecalcularTodaLaBaseDeDatos()
        {
            var listaBLLs = ObtenerBLLsVerificables();
            foreach (var bll in listaBLLs)
            {
                bll.RecalcularIntegridad();
            }
            RegistrarEventoBitacora("Se forzó el recálculo masivo de los dígitos verificadores (DVH/DVV) de toda la base de datos.", 3);
        }
        public void RegistrarEventoIntegridadComprometida(string error)
        {
            RegistrarEventoBitacora($"Alerta de Integridad:\n{error}", 3);
        }

        public string VerificarTodaLaBaseDeDatos()
        {
            var reporteErrores = new StringBuilder();
            List<DigitoVerificador> dvGuardados = this.Listar();
            var listaBLLs = ObtenerBLLsVerificables();

            foreach (var bll in listaBLLs)
            {
                string nombreTabla = bll.ObtenerNombreTabla();
                ReporteIntegridad reporteActual = bll.CalcularIntegridadActual();

                if (reporteActual.DVH_Actual == "0") continue;

                var dvMaestroGuardado = dvGuardados.FirstOrDefault(dv => dv.NombreTabla == nombreTabla);

                if (dvMaestroGuardado == null)
                {
                    reporteErrores.AppendLine($"ERROR CRÍTICO: La tabla '{nombreTabla}' no tiene DV guardados en el sistema.");
                    continue;
                }

                // Si la DAL detectó filas alteradas internamente, las volcamos al reporte.
                foreach (var errorFila in reporteActual.ErroresDetallados)
                {
                    reporteErrores.AppendLine(errorFila);
                }

                // MATEMÁTICA FORENSE: Detección de registros borrados o insertados fantasma
                if (dvMaestroGuardado.DVH != reporteActual.DVH_Actual && reporteActual.EsValido)
                {
                    // Entra acá si el Total no coincide, pero NINGUNA fila existente está corrupta.
                    try
                    {
                        BigInteger guardadoNum = BigInteger.Parse("00" + dvMaestroGuardado.DVH, NumberStyles.HexNumber);
                        BigInteger actualNum = BigInteger.Parse("00" + reporteActual.DVH_Actual, NumberStyles.HexNumber);

                        if (guardadoNum > actualNum)
                        {
                            BigInteger diferencia = guardadoNum - actualNum;
                            reporteErrores.AppendLine($"- REGISTRO BORRADO en '{nombreTabla}': Se eliminó una fila por fuera del sistema. El Hash exacto del registro eliminado era: {diferencia.ToString("X")}");
                        }
                        else
                        {
                            reporteErrores.AppendLine($"- REGISTRO INYECTADO en '{nombreTabla}': Se insertó un registro directamente en SQL. Corrupción detectada.");
                        }
                    }
                    catch { }
                }
            }

            if (reporteErrores.Length == 0)
            {
                return "OK. La integridad de la base de datos es 100% correcta.";
            }

            string logError = $"Alerta de Integridad Detallada:\n{reporteErrores.ToString()}";
            RegistrarEventoBitacora(logError, 3);

            return logError;
        }

        private List<IGestorIntegridad> ObtenerBLLsVerificables()
        {
            var listaInstancias = new List<IGestorIntegridad>();

            var tiposVerificables = Assembly.GetExecutingAssembly().GetTypes().Where(tipo => typeof(IGestorIntegridad).IsAssignableFrom(tipo) && !tipo.IsInterface && !tipo.IsAbstract && tipo != typeof(BLL_DigitoVerificador)); 

            foreach (var tipo in tiposVerificables)
            {
                var instancia = (IGestorIntegridad)Activator.CreateInstance(tipo);
                listaInstancias.Add(instancia);
            }

            return listaInstancias;
        }

        private void RegistrarEventoBitacora(string mensaje, int criticidad)
        {
            try
            {
                string usuario = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
                _bllEvento.Registrar(usuario, ModulosEventos.BaseDeDatos, mensaje, criticidad);
            }
            catch
            {
            }
        }

    }
}
