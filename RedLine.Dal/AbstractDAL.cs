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
        protected string cx = "cadena coming soon";

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

        protected virtual string HashearTexto(string texto)
        {
            return RedLine.Servicios.Hashing.Sha256(texto);
        }

        #endregion

        #region Operaciones Base de datos

        public virtual void Insertar(entidad entidad)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(SqlInsertar, cmd => ConfigurarParametros(cmd, entidad));
                if (RequiereDigitoVerificador) RecalcularDigitosVerificadores(con);
            }
        }

        public virtual void Modificar(entidad entidad)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(SqlModificar, cmd => ConfigurarParametros(cmd, entidad));
                if (RequiereDigitoVerificador) RecalcularDigitosVerificadores(con);
            }
        }


        public virtual void Eliminar(TKey id)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(SqlEliminar, cmd => ConfigurarParametrosId(cmd, id));
                if (RequiereDigitoVerificador) RecalcularDigitosVerificadores(con);
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

        #region Motor Digito Verificador 
        // Dispara el cálculo y guardado de los dígitos
        private void RecalcularDigitosVerificadores(SqlConnection con)
        {
            var (dvh, dvv) = CalcularChecksums(con);
            GuardarDigitosVerificadores(con, NombreTabla, dvh, dvv);
        }
        // Matematica de seguridad: Suma de hash de cada campo por fila (DVH) y suma de hash de cada campo por columna (DVV)
        private (string DVH, string DVV) CalcularChecksums(SqlConnection con)
        {
            List<string> columnasOrdenadas = ObtenerColumnasOrdenadas(con, NombreTabla);
            if (columnasOrdenadas.Count == 0) return ("0", "0");

            string columnasQuery = string.Join(", ", columnasOrdenadas.Select(c => $"[{c}]"));
            string query = $"SELECT {columnasQuery} FROM [{NombreTabla}]";

            List<object[]> registros = new List<object[]>();
            int fieldCount = columnasOrdenadas.Count;

            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    object[] fila = new object[fieldCount];
                    rdr.GetValues(fila);
                    registros.Add(fila);
                }
            }

            if (registros.Count == 0) return ("0", "0");

            BigInteger sumaTotalHorizontal = 0;
            BigInteger sumaTotalVertical = 0;

            // Calculo Horizontal
            foreach (var fila in registros)
            {
                BigInteger sumaParcialFila = 0;
                foreach (object o in fila)
                {
                    string hex = HashearTexto(o?.ToString() ?? "");
                    sumaParcialFila += BigInteger.Parse("00" + hex, NumberStyles.HexNumber);
                }
                string hexFila = HashearTexto(sumaParcialFila.ToString());
                sumaTotalHorizontal += BigInteger.Parse("00" + hexFila, NumberStyles.HexNumber);
            }

            // Calculo Vertical
            for (int col = 0; col < fieldCount; col++)
            {
                BigInteger sumaColumna = 0;
                foreach (var fila in registros)
                {
                    string hex = HashearTexto(fila[col]?.ToString() ?? "");
                    sumaColumna += BigInteger.Parse("00" + hex, NumberStyles.HexNumber);
                }
                string hexCol = HashearTexto(sumaColumna.ToString());
                sumaTotalVertical += BigInteger.Parse("00" + hexCol, NumberStyles.HexNumber);
            }

            return (sumaTotalHorizontal.ToString("X"), sumaTotalVertical.ToString("X"));
        }
        // Obtiene la estructura de la tabla excluyendo timestamps
        private List<string> ObtenerColumnasOrdenadas(SqlConnection con, string nombreTabla)
        {
            var columnas = new List<string>();
            string queryMeta = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
                                 WHERE TABLE_NAME = @NombreTabla 
                                 AND DATA_TYPE NOT IN ('timestamp', 'rowversion')
                                 ORDER BY COLUMN_NAME ASC";

            using (var cmd = new SqlCommand(queryMeta, con))
            {
                cmd.AgregarParametro("@NombreTabla", nombreTabla);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read()) columnas.Add(rdr["COLUMN_NAME"].ToString());
                }
            }
            return columnas;
        }
        // Inserta o actualiza los códigos en la tabla DigitoVerificador
        private void GuardarDigitosVerificadores(SqlConnection con, string nombreTabla, string checksumHorizontal, string checksumVertical)
        {
            // LA CLASE DigitoVerificador DEBE EXISTIR EN LA BASE DE DATOS CON LOS CAMPOS: NombreTabla (PK), DVH, DVV
            string queryCheck = "SELECT COUNT(*) FROM DigitoVerificador WHERE NombreTabla = @nombre";
            int existe = con.EjecutarEscalar<int>(queryCheck, c => c.AgregarParametro("@nombre", nombreTabla));

            if (existe > 0)
            {
                string queryUpdate = "UPDATE DigitoVerificador SET DVH = @horizontal, DVV = @vertical WHERE NombreTabla = @nombre";
                con.EjecutarNoConsulta(queryUpdate, c =>
                {
                    c.AgregarParametro("@horizontal", checksumHorizontal);
                    c.AgregarParametro("@vertical", checksumVertical);
                    c.AgregarParametro("@nombre", nombreTabla);
                });
            }
            else
            {
                string queryInsert = "INSERT INTO DigitoVerificador (NombreTabla, DVH, DVV) VALUES (@nombre, @horizontal, @vertical)";
                con.EjecutarNoConsulta(queryInsert, c =>
                {
                    c.AgregarParametro("@nombre", nombreTabla);
                    c.AgregarParametro("@horizontal", checksumHorizontal);
                    c.AgregarParametro("@vertical", checksumVertical);
                });
            }
        }
        #endregion
    }

}
