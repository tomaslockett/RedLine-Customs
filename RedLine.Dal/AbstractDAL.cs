using RedLine.Be.Interfaces;
using RedLine.Dal.ORM;
using RedLine.Entidades;
using RedLine.Servicios;
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
    public abstract class AbstractDAL<TKey, entidad> : IRepositorioBasico<TKey, entidad>, IGestorIntegridad
    {
        protected string cx = "Data Source=.;Initial Catalog=RedLineCustomsDB;Integrated Security=True";

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
            ReporteIntegridad reporte = CalcularIntegridadActual(con, tra);

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
                cmdUpsert.Parameters.AddWithValue("@dvh", reporte.DVH_Actual);
                cmdUpsert.Parameters.AddWithValue("@dvv", reporte.DVV_Actual);
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

        public virtual ReporteIntegridad CalcularIntegridadActual()
        {
            if (!RequiereDigitoVerificador) return new ReporteIntegridad();

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                return CalcularIntegridadActual(con, null);
            }
        }

        private ReporteIntegridad CalcularIntegridadActual(SqlConnection con, SqlTransaction tra)
        {
            var reporte = new ReporteIntegridad();
            if (!RequiereDigitoVerificador) return reporte;

            var columnasDatos = new List<string>();
            bool tablaTieneDVH = false;

            // 1. Obtenemos todas las columnas válidas
            string queryMeta = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @Tabla AND DATA_TYPE NOT IN ('timestamp', 'rowversion') ORDER BY COLUMN_NAME ASC";
            using (var cmdMeta = new SqlCommand(queryMeta, con, tra))
            {
                cmdMeta.Parameters.AddWithValue("@Tabla", this.NombreTabla);
                using (var rdrMeta = cmdMeta.ExecuteReader())
                {
                    while (rdrMeta.Read())
                    {
                        string col = rdrMeta["COLUMN_NAME"].ToString();
                        if (col.ToUpper() == "DVH") tablaTieneDVH = true;
                        else columnasDatos.Add(col);
                    }
                }
            }

            if (columnasDatos.Count == 0) return reporte;

            // 🚨 2. EL FIX: BUSCAMOS LA VERDADERA PRIMARY KEY EN SQL SERVER
            string idColumna = columnasDatos.First(); // Fallback por si la tabla no tiene PK
            string queryPK = @"
        SELECT kcu.COLUMN_NAME 
        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc 
        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu 
          ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME 
        WHERE tc.TABLE_NAME = @Tabla AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'";

            using (var cmdPK = new SqlCommand(queryPK, con, tra))
            {
                cmdPK.Parameters.AddWithValue("@Tabla", this.NombreTabla);
                var pkObj = cmdPK.ExecuteScalar();
                if (pkObj != null) idColumna = pkObj.ToString();
            }

            // Le pasamos las columnas al motor
            var motor = new RedLine.Servicios.MotorDigitoVerificador(columnasDatos);
            reporte.NombreColumnaID = idColumna; // Guardamos el nombre real de la PK (ej: "ID")

            string columnasQuery = string.Join(", ", columnasDatos.Select(c => $"[{c}]"));
            if (tablaTieneDVH) columnasQuery += ", [DVH]";

            string queryData = $"SELECT {columnasQuery} FROM [{this.NombreTabla}]";

            using (var cmdData = new SqlCommand(queryData, con, tra))
            using (var rdrData = cmdData.ExecuteReader())
            {
                while (rdrData.Read())
                {
                    string[] filaTexto = new string[columnasDatos.Count];
                    for (int i = 0; i < columnasDatos.Count; i++)
                    {
                        filaTexto[i] = rdrData[i]?.ToString() ?? "";
                    }

                    // AHORA SÍ: Capturamos el ID Real y Único
                    string idFilaActual = rdrData[idColumna]?.ToString();
                    string hashCalculado = motor.ProcesarFila(idFilaActual, filaTexto);

                    if (tablaTieneDVH)
                    {
                        string hashGuardado = rdrData["DVH"]?.ToString();
                        if (hashCalculado != hashGuardado)
                        {
                            reporte.FilasCorruptasIDs.Add(idFilaActual);
                        }
                    }
                }
            }

            var (dvh, dvv) = motor.ObtenerResultadoFinal();
            reporte.DVH_Actual = dvh;
            reporte.DVV_Actual = dvv;

            return reporte;
        }

        public virtual void RecalcularIntegridad()
        {
            this.RecalcularMisDigitosVerificadores();
        }

        #endregion

    }
}
