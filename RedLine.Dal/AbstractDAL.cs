using RedLine.Be.Interfaces;
using RedLine.Dal.ORM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal
{
    // ⚠️ ATENCIÓN REQUISITO DE BD: 
    // Para que este motor funcione, DEBE existir en la base de datos una tabla 
    // llamada exactamente 'DigitoVerificador' con las columnas: NombreTabla (PK), DVH, DVV.
    public abstract class AbstractDAL<TKey, entidad> : IRepositorioBasico<TKey, entidad>
    {
        protected string cx = "Data Source=.;Initial Catalog=\"Redline Customs\";Integrated Security=True;Encrypt=False;";

        #region Configuracion
        protected abstract string NombreTabla { get; }
        protected abstract bool RequiereDigitoVerificador { get; }
        protected abstract string SqlInsertar { get; }
        protected abstract string SqlModificar { get; }
        protected abstract string SqlEliminar { get; }
        protected abstract string SqlListar { get; }
        protected abstract string SqlObtenerPorId { get; }

        protected abstract void ConfigurarParametros(SqlCommand cmd, entidad entidad);
        protected abstract void ConfigurarParametrosId(SqlCommand cmd, TKey id);
        protected abstract entidad Mapear(SqlDataReader lector);

        #endregion

        #region Operaciones Base de Datos con Transacción

        public virtual void Insertar(entidad entidad)
        {
            EjecutarOperacionConIntegridad(SqlInsertar, cmd => ConfigurarParametros(cmd, entidad));
        }

        public virtual void Modificar(entidad entidad)
        {
            EjecutarOperacionConIntegridad(SqlModificar, cmd => ConfigurarParametros(cmd, entidad));
        }

        public virtual void Eliminar(TKey id)
        {
            EjecutarOperacionConIntegridad(SqlEliminar, cmd => ConfigurarParametrosId(cmd, id));
        }

        private void EjecutarOperacionConIntegridad(string query, Action<SqlCommand> configurar)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction()) 
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, con, tra))
                        {
                            configurar(cmd);
                            cmd.ExecuteNonQuery();
                        }

                        if (RequiereDigitoVerificador)
                        {
                            this.RecalcularMisDigitosVerificadores(con, tra);
                        }

                        tra.Commit(); 
                    }
                    catch (Exception)
                    {
                        tra.Rollback(); 
                        throw; 
                    }
                }
            }
        }

        public virtual List<entidad> Listar()
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                return con.EjecutarLectura(SqlListar, Mapear);
            }
        }

        public virtual entidad ObtenerPorId(TKey id)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                var lista = con.EjecutarLectura(SqlObtenerPorId, Mapear, cmd => ConfigurarParametrosId(cmd, id));
                return lista.FirstOrDefault();
            }
        }

        public abstract entidad ObtenerPorEntidad(entidad entidad);

        #endregion

        #region Motor de Dígito Verificador 

        public virtual string ObtenerNombreTabla() => this.NombreTabla;

        public virtual void RecalcularMisDigitosVerificadores(SqlConnection con, SqlTransaction tra)
        {
            var (dvh, dvv) = CalcularIntegridadActual(con, tra);

            string queryCheck = "SELECT COUNT(*) FROM DigitoVerificador WHERE NombreTabla = @nombre";
            int existe;
            using (var cmdCheck = new SqlCommand(queryCheck, con, tra))
            {
                cmdCheck.Parameters.AddWithValue("@nombre", this.NombreTabla);
                existe = (int)cmdCheck.ExecuteScalar();
            }

            string queryUpsert = existe > 0
                ? "UPDATE DigitoVerificador SET DVH = @dvh, DVV = @dvv WHERE NombreTabla = @nombre"
                : "INSERT INTO DigitoVerificador (NombreTabla, DVH, DVV) VALUES (@nombre, @dvh, @dvv)";

            using (var cmdUpsert = new SqlCommand(queryUpsert, con, tra))
            {
                cmdUpsert.Parameters.AddWithValue("@dvh", dvh);
                cmdUpsert.Parameters.AddWithValue("@dvv", dvv);
                cmdUpsert.Parameters.AddWithValue("@nombre", this.NombreTabla);
                cmdUpsert.ExecuteNonQuery();
            }
        }

        public virtual void RecalcularMisDigitosVerificadores()
        {
            if (!RequiereDigitoVerificador) return;

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                this.RecalcularMisDigitosVerificadores(con, null);
            }
        }
        public virtual (string DVH, string DVV) CalcularIntegridadActual()
        {
            if (!RequiereDigitoVerificador) return ("N/A", "N/A");

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                return CalcularIntegridadActual(con, null);
            }
        }

        private (string DVH, string DVV) CalcularIntegridadActual(SqlConnection con, SqlTransaction tra)
        {
            if (!RequiereDigitoVerificador) return ("N/A", "N/A");

            var columnas = new List<string>();
            string queryMeta = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @Tabla AND DATA_TYPE NOT IN ('timestamp', 'rowversion') ORDER BY COLUMN_NAME ASC";

            using (var cmdMeta = new SqlCommand(queryMeta, con, tra))
            {
                cmdMeta.Parameters.AddWithValue("@Tabla", this.NombreTabla);
                using (var rdrMeta = cmdMeta.ExecuteReader())
                {
                    while (rdrMeta.Read()) columnas.Add(rdrMeta["COLUMN_NAME"].ToString());
                }
            }

            if (columnas.Count == 0) return ("0", "0");

            var motor = new RedLine.Servicios.MotorDigitoVerificador(columnas.Count);
            string columnasQuery = string.Join(", ", columnas.Select(c => $"[{c}]"));
            string queryData = $"SELECT {columnasQuery} FROM [{this.NombreTabla}]";

            using (var cmdData = new SqlCommand(queryData, con, tra))
            using (var rdrData = cmdData.ExecuteReader())
            {
                while (rdrData.Read())
                {
                    string[] filaTexto = new string[columnas.Count];
                    for (int i = 0; i < columnas.Count; i++)
                    {
                        filaTexto[i] = rdrData[i]?.ToString() ?? "";
                    }
                    motor.ProcesarFila(filaTexto);
                }
            }

            return motor.ObtenerResultadoFinal();
        }

        #endregion

    }
}
