using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal
{
    public class DAL_BackupRestore : AbstractDAL<int, object>
    {
        protected override string NombreTabla => ""; 
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar => "";
        protected override string SqlModificar => "";
        protected override string SqlEliminar => "";
        protected override string SqlListar => "";
        protected override string SqlObtenerPorId => "";
        protected override void ConfigurarParametros(SqlCommand cmd, object entidad) { }
        protected override void ConfigurarParametrosId(SqlCommand cmd, int id) { }
        protected override object Mapear(SqlDataReader lector) => null;
        public override object ObtenerPorEntidad(object entidad) => null;

        public void RealizarBackup(string rutaCompleta)
        {
            string comando = $"BACKUP DATABASE [Redline Customs] TO DISK = '{rutaCompleta}' WITH FORMAT";

            using (SqlConnection con = new SqlConnection(cx))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(comando, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RealizarRestore(string rutaArchivo)
        {
            using (SqlConnection con = new SqlConnection(cx))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("USE master;", con)) { cmd.ExecuteNonQuery(); }

                string sqlSingle = "ALTER DATABASE [Redline Customs] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                using (SqlCommand cmd = new SqlCommand(sqlSingle, con)) { cmd.ExecuteNonQuery(); }

                string sqlRestore = $"RESTORE DATABASE [Redline Customs] FROM DISK = '{rutaArchivo}' WITH REPLACE;";
                using (SqlCommand cmd = new SqlCommand(sqlRestore, con)) { cmd.ExecuteNonQuery(); }

                string sqlMulti = "ALTER DATABASE [Redline Customs] SET MULTI_USER;";
                using (SqlCommand cmd = new SqlCommand(sqlMulti, con)) { cmd.ExecuteNonQuery(); }
            }
        }
    }
}
