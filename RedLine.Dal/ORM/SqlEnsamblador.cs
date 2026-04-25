using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.ORM
{
    public static class SqlEnsamblador
    {
        public static SqlParameter AgregarParametro(this SqlCommand comando, string nombre, object valor)
        {
            return comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public static SqlParameter AgregarParametroDecimal(this SqlCommand comando, string nombre, decimal? valor, byte precision = 18, byte escala = 2)
        {
            var p = comando.Parameters.Add(nombre, SqlDbType.Decimal);
            p.Precision = precision;
            p.Scale = escala;
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        public static SqlParameter AgregarParametroFecha(this SqlCommand comando, string nombre, DateTime? valor)
        {
            var p = comando.Parameters.Add(nombre, SqlDbType.DateTime);
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        public static SqlParameter AgregarParametroBooleano(this SqlCommand comando, string nombre, bool? valor)
        {
            var p = comando.Parameters.Add(nombre, SqlDbType.Bit);
            p.Value = valor.HasValue ? (object)valor.Value : DBNull.Value;
            return p;
        }

        // Lectura
        public static T Obtener<T>(this SqlDataReader lector, string columna)
        {
            int indice = lector.GetOrdinal(columna);
            if (lector.IsDBNull(indice))
            {
                return default(T); 
            }

            object valor = lector.GetValue(indice);
            Type tipo = typeof(T);
            Type subTipo = Nullable.GetUnderlyingType(tipo) ?? tipo;

            // Soporte nativo para Enums
            if (subTipo.IsEnum)
            {
                return (T)Enum.ToObject(subTipo, valor);
            }

            return (T)Convert.ChangeType(valor, subTipo);
        }

        public static string ObtenerCadenaSegura(this SqlDataReader lector, string columna)
        {
            int indice = lector.GetOrdinal(columna);
            return lector.IsDBNull(indice) ? string.Empty : lector.GetString(indice);
        }

        // Ejecución
        public static int EjecutarNoConsulta(this SqlConnection conexion, string sql, Action<SqlCommand> configurarParametros = null)
        {
            using (var comando = new SqlCommand(sql, conexion))
            {
                configurarParametros?.Invoke(comando);
                return comando.ExecuteNonQuery();
            }
        }

        public static T EjecutarEscalar<T>(this SqlConnection conexion, string sql, Action<SqlCommand> configurarParametros = null)
        {
            using (var comando = new SqlCommand(sql, conexion))
            {
                configurarParametros?.Invoke(comando);
                var resultado = comando.ExecuteScalar();

                if (resultado == null || resultado == DBNull.Value)
                {
                    return default(T); 
                }
                return (T)Convert.ChangeType(resultado, typeof(T));
            }
        }

        public static List<T> EjecutarLectura<T>(this SqlConnection conexion, string sql, Func<SqlDataReader, T> mapear, Action<SqlCommand> configurarParametros = null)
        {
            var lista = new List<T>();
            using (var comando = new SqlCommand(sql, conexion))
            {
                configurarParametros?.Invoke(comando);

                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(mapear(lector));
                    }
                }
            }
            return lista;
        }
    }
}
