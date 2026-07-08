using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_BackupRestore
    {
        private DAL_BackupRestore _dal;
        private BLL_Evento _bllEvento = new BLL_Evento();

        public BLL_BackupRestore()
        {
            _dal = new DAL_BackupRestore();
        }

        public void RealizarBackup(string backupPath)
        {
            if (!Directory.Exists(backupPath))
                throw new Exception("La carpeta de destino no existe.");

            string nombreArchivo = $"BCK_{DateTime.Now:ddMMyy_HHmm}.bak";
            string rutaFinal = Path.Combine(backupPath, nombreArchivo);

            _dal.RealizarBackup(rutaFinal);
            string user = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(user, "Base de Datos", $"Copia de seguridad generada con éxito: {nombreArchivo}", 2);
        }

        public void RealizarRestore(string restorePath)
        {
            if (!File.Exists(restorePath))
                throw new Exception("El archivo de restauración no existe.");

            if (!Path.GetExtension(restorePath).Equals(".bak", StringComparison.OrdinalIgnoreCase))
                throw new Exception("El archivo seleccionado no tiene un formato válido (.bak).");

            try
            {
                string carpetaContenedora = Path.GetDirectoryName(restorePath);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "icacls",
                    Arguments = $"\"{carpetaContenedora}\" /grant \"NT SERVICE\\MSSQLSERVER\":(OI)(CI)RX /T",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process proceso = Process.Start(psi))
                {
                    proceso.WaitForExit();
                }
            }
            catch (Exception)
            {

            }

            _dal.RealizarRestore(restorePath);
            string user = SessionManager.Instancia.Usuario.Email;
            string nombreArchivo = Path.GetFileName(restorePath);
            _bllEvento.Registrar(user, "Base de Datos", $"Restauración completa del sistema realizada desde: {nombreArchivo}", 3);
        }
    }
}
