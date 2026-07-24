using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Diagnostics;
using System.IO;

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
            {
                throw new Exception("La carpeta de destino no existe.");
            }

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "icacls",
                    Arguments = $"\"{backupPath}\" /grant \"NT SERVICE\\MSSQLSERVER\":(OI)(CI)F /T",
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
            catch { }

            string nombreArchivo = $"BCK_{DateTime.Now:ddMMyy_HHmm}.bak";
            string rutaFinal = Path.Combine(backupPath, nombreArchivo);

            _dal.RealizarBackup(rutaFinal);

            RegistrarEventoBitacora($"Copia de seguridad generada con éxito: {nombreArchivo}", 2);
        }

        public void RealizarRestore(string restorePath)
        {
            if (!File.Exists(restorePath))
            {
                throw new Exception("El archivo de restauración no existe.");
            }

            if (!Path.GetExtension(restorePath).Equals(".bak", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("El archivo seleccionado no tiene un formato válido (.bak).");
            }

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
            string nombreArchivo = Path.GetFileName(restorePath);

            RegistrarEventoBitacora($"Restauración completa del sistema realizada desde: {nombreArchivo}", 3);
        }

        #region Métodos Privados

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

        #endregion
    }
}
