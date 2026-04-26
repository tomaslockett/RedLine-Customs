using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace RedLine.Servicios
{
    public static class DigitoVerificador
    {
        public static void Recalcular(SqlConnection con, string nombreTabla)
        {
            var (dvh, dvv) = CalcularChecksums(con, nombreTabla);
            GuardarDigitosVerificadores(con, nombreTabla, dvh, dvv);
        }

        private static (string DVH, string DVV) CalcularChecksums(SqlConnection con, string nombreTabla)
        {
            List<string> columnasOrdenadas = ObtenerColumnasOrdenadas(con, nombreTabla);
            if (columnasOrdenadas.Count == 0) return ("0", "0");

            string columnasQuery = string.Join(", ", columnasOrdenadas.Select(c => $"[{c}]"));
            string query = $"SELECT {columnasQuery} FROM [{nombreTabla}]";

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
                    string hex = Hashing.Sha256(o?.ToString() ?? "");
                    sumaParcialFila += BigInteger.Parse("00" + hex, NumberStyles.HexNumber);
                }
                string hexFila = Hashing.Sha256(sumaParcialFila.ToString());
                sumaTotalHorizontal += BigInteger.Parse("00" + hexFila, NumberStyles.HexNumber);
            }

            // Calculo Vertical
            for (int col = 0; col < fieldCount; col++)
            {
                BigInteger sumaColumna = 0;
                foreach (var fila in registros)
                {
                    string hex = Hashing.Sha256(fila[col]?.ToString() ?? "");
                    sumaColumna += BigInteger.Parse("00" + hex, NumberStyles.HexNumber);
                }
                string hexCol = Hashing.Sha256(sumaColumna.ToString());
                sumaTotalVertical += BigInteger.Parse("00" + hexCol, NumberStyles.HexNumber);
            }

            return (sumaTotalHorizontal.ToString("X"), sumaTotalVertical.ToString("X"));
        }

        private static List<string> ObtenerColumnasOrdenadas(SqlConnection con, string nombreTabla)
        {
            var columnas = new List<string>();
            string queryMeta = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
                                 WHERE TABLE_NAME = @NombreTabla 
                                 AND DATA_TYPE NOT IN ('timestamp', 'rowversion')
                                 ORDER BY COLUMN_NAME ASC";

            using (var cmd = new SqlCommand(queryMeta, con))
            {
                cmd.Parameters.AddWithValue("@NombreTabla", nombreTabla);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read()) columnas.Add(rdr["COLUMN_NAME"].ToString());
                }
            }
            return columnas;
        }

        //
        //  VERIFICAR LA TABLA DE DIGITO VERIFICADOR, SI EXISTE EL REGISTRO DE LA TABLA, HACER UPDATE, SINO HACER INSERT
        //

        private static void GuardarDigitosVerificadores(SqlConnection con, string nombreTabla, string checksumHorizontal, string checksumVertical)
        {
            string queryCheck = "SELECT COUNT(*) FROM DigitoVerificador WHERE NombreTabla = @nombre";
            int existe = 0;

            using (var cmdCheck = new SqlCommand(queryCheck, con))
            {
                cmdCheck.Parameters.AddWithValue("@nombre", nombreTabla);
                existe = (int)cmdCheck.ExecuteScalar();
            }

            if (existe > 0)
            {
                string queryUpdate = "UPDATE DigitoVerificador SET DVH = @horizontal, DVV = @vertical WHERE NombreTabla = @nombre";
                using (var cmdUpd = new SqlCommand(queryUpdate, con))
                {
                    cmdUpd.Parameters.AddWithValue("@horizontal", checksumHorizontal);
                    cmdUpd.Parameters.AddWithValue("@vertical", checksumVertical);
                    cmdUpd.Parameters.AddWithValue("@nombre", nombreTabla);
                    cmdUpd.ExecuteNonQuery();
                }
            }
            else
            {
                string queryInsert = "INSERT INTO DigitoVerificador (NombreTabla, DVH, DVV) VALUES (@nombre, @horizontal, @vertical)";
                using (var cmdIns = new SqlCommand(queryInsert, con))
                {
                    cmdIns.Parameters.AddWithValue("@nombre", nombreTabla);
                    cmdIns.Parameters.AddWithValue("@horizontal", checksumHorizontal);
                    cmdIns.Parameters.AddWithValue("@vertical", checksumVertical);
                    cmdIns.ExecuteNonQuery();
                }
            }
        }
    }
}
