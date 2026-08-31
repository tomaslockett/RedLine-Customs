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
            RegistrarEventoBitacora($"Recálculo forzado de la tabla: {nombreTabla}", 2);
        }

        public void RecalcularTodaLaBaseDeDatos()
        {
            var listaBLLs = ObtenerBLLsVerificables();
            foreach (var bll in listaBLLs)
            {
                bll.RecalcularIntegridad();
            }
        }

        public void RegistrarEventoIntegridadComprometida(string error)
        {
            RegistrarEventoBitacora($"Alerta de Integridad Histórica:\n{error}", 3);
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

                var dvMaestroGuardado = dvGuardados.FirstOrDefault(dv => dv.NombreTabla == nombreTabla);
                if (dvMaestroGuardado == null) continue;

                var dicDvvGuardado = ParsearMatriz(dvMaestroGuardado.DVV);
                var dicDvvActual = ParsearMatriz(reporteActual.DVV_Actual);

                var dicDvhGuardado = ParsearMatriz(dvMaestroGuardado.DVH);
                var dicDvhActual = ParsearMatriz(reporteActual.DVH_Actual);


                List<string> colsCambiadas = dicDvvActual.Keys.Where(k => dicDvvGuardado.ContainsKey(k) && dicDvvGuardado[k] != dicDvvActual[k]).ToList();
                string colStrings = colsCambiadas.Count > 0 ? string.Join(", ", colsCambiadas) : "Ninguna";


                List<string> filasBorradas = dicDvhGuardado.Keys.Where(k => !dicDvhActual.ContainsKey(k)).ToList();
                List<string> filasInsertadas = dicDvhActual.Keys.Where(k => !dicDvhGuardado.ContainsKey(k)).ToList();
                List<string> filasModificadas = dicDvhActual.Keys.Where(k => dicDvhGuardado.ContainsKey(k) && dicDvhGuardado[k] != dicDvhActual[k]).ToList();


                bool huboCorrupcion = false;

                if (filasModificadas.Count > 0)
                {
                    reporteErrores.AppendLine($"MODIFICACIÓN en '{nombreTabla}': Se alteró la FILA [ID: {string.Join(", ", filasModificadas)}] en las COLUMNAS [{colStrings}].");
                    huboCorrupcion = true;
                }

                if (filasBorradas.Count > 0)
                {
                    reporteErrores.AppendLine($" ELIMINACIÓN en '{nombreTabla}': Se borró por completo la FILA [ID: {string.Join(", ", filasBorradas)}].");
                    huboCorrupcion = true;
                }

                if (filasInsertadas.Count > 0)
                {
                    reporteErrores.AppendLine($" INSERCIÓN en '{nombreTabla}': Se agregó sin permiso la FILA [ID: {string.Join(", ", filasInsertadas)}].");
                    huboCorrupcion = true;
                }

                // 5. DETECCIÓN DE ATAQUE AL MAESTRO (Si nadie tocó las filas, pero los DVs globales no dan)
                if (!huboCorrupcion && (colsCambiadas.Count > 0 || dvMaestroGuardado.DVH != reporteActual.DVH_Actual))
                {
                    reporteErrores.AppendLine($"CORRUPCIÓN en '{nombreTabla}': Discrepancia en las firmas. Posible manipulación directa de la tabla DigitoVerificador.");
                }
            }

            if (reporteErrores.Length == 0) return "OK. La integridad de la base de datos es 100% correcta.";

            return $"Alerta de Integridad Detallada:\n{reporteErrores.ToString()}";
        }

        private Dictionary<string, string> ParsearMatriz(string cadenaMatricial)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(cadenaMatricial)) return dic;

            foreach (var par in cadenaMatricial.Split('|'))
            {
                var partes = par.Split(':');
                if (partes.Length == 2) dic[partes[0]] = partes[1];
            }
            return dic;
        }

        private List<IGestorIntegridad> ObtenerBLLsVerificables()
        {
            var listaInstancias = new List<IGestorIntegridad>();
            var tiposVerificables = Assembly.GetExecutingAssembly().GetTypes().Where(tipo => typeof(IGestorIntegridad).IsAssignableFrom(tipo) && !tipo.IsInterface && !tipo.IsAbstract && tipo != typeof(BLL_DigitoVerificador));

            foreach (var tipo in tiposVerificables)
            {
                listaInstancias.Add((IGestorIntegridad)Activator.CreateInstance(tipo));
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
            catch { }
        }
    }
}
