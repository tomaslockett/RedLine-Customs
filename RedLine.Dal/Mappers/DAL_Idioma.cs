using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.Mappers
{
    public class DAL_Idioma : AbstractDAL<int, Idioma>
    {
        protected override string NombreTabla => "Idioma";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar => @"INSERT INTO Idioma (Nombre, EsDefault) VALUES (@Nombre, @EsDefault)";
        protected override string SqlModificar => @"UPDATE Idioma SET Nombre = @Nombre, EsDefault = @EsDefault WHERE Id = @Id";
        protected override string SqlEliminar => @"DELETE FROM Idioma WHERE Id = @Id";
        protected override string SqlListar => @"SELECT * FROM Idioma";
        protected override string SqlObtenerPorId => @"SELECT * FROM Idioma WHERE Id = @Id";

        protected override void ConfigurarParametros(SqlCommand cmd, Idioma entidad)
        {
            if (cmd.CommandText.Contains("@Id"))
            {
                cmd.Parameters.AddWithValue("@Id", entidad.Id);
            }
            cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@EsDefault", entidad.EsDefault);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@Id", id);
        }

        protected override Idioma Mapear(SqlDataReader lector)
        {
            return new Idioma(
                Convert.ToInt32(lector["Id"]),
                lector["Nombre"].ToString(),
                Convert.ToBoolean(lector["EsDefault"])
            );
        }

        public override Idioma ObtenerPorEntidad(Idioma entidad)
        {
            return ObtenerPorId(entidad.Id);
        }

        public Dictionary<string, string> ObtenerTraduccionesPorIdioma(int idIdioma)
        {
            var traducciones = new Dictionary<string, string>();
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = @"
            SELECT E.Clave, COALESCE(T.Texto, '') AS Texto
            FROM (SELECT DISTINCT Clave FROM Traduccion) E
            LEFT JOIN Traduccion T ON E.Clave = T.Clave AND T.IdIdioma = @IdIdioma";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            traducciones[reader["Clave"].ToString()] = reader["Texto"].ToString();
                        }
                    }
                }
            }
            return traducciones;
        }

        public Dictionary<string, string> ObtenerTraduccionesConFallback(int idIdioma)
        {
            var traducciones = new Dictionary<string, string>();
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = @"
            SELECT E.Clave, 
                   COALESCE(NULLIF(T.Texto, ''), '[' + E.Clave + ']') AS Texto
            FROM (SELECT DISTINCT Clave FROM Traduccion) E
            LEFT JOIN Traduccion T ON E.Clave = T.Clave AND T.IdIdioma = @IdIdioma";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            traducciones[reader["Clave"].ToString()] = reader["Texto"].ToString();
                        }
                    }
                }
            }
            return traducciones;
        }

        public Idioma ObtenerDefault()
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = "SELECT TOP 1 * FROM Idioma WHERE EsDefault = 1";
                using (var cmd = new SqlCommand(query, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return Mapear(reader);
                    }
                }
            }
            return null;
        }

        public void GuardarTraduccion(int idIdioma, string clave, string texto)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = @"IF EXISTS (SELECT 1 FROM Traduccion WHERE IdIdioma = @IdIdioma AND Clave = @Clave)
                                    UPDATE Traduccion SET Texto = @Texto WHERE IdIdioma = @IdIdioma AND Clave = @Clave
                                 ELSE
                                    INSERT INTO Traduccion (IdIdioma, Clave, Texto) VALUES (@IdIdioma, @Clave, @Texto)";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);
                    cmd.Parameters.AddWithValue("@Clave", clave);
                    cmd.Parameters.AddWithValue("@Texto", texto);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
