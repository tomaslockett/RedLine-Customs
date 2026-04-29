using RedLine.Dal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_BackupRestore
    {
        private DAL_BackupRestore _dal;

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
        }

        public void RealizarRestore(string restorePath)
        {
            if (!File.Exists(restorePath))
                throw new Exception("El archivo de restauración no existe.");

            if (!Path.GetExtension(restorePath).Equals(".bak", StringComparison.OrdinalIgnoreCase))
                throw new Exception("El archivo seleccionado no tiene un formato válido (.bak).");

            _dal.RealizarRestore(restorePath);
        }
    }
}
