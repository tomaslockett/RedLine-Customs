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
        protected override string SqlModificar => @"UPDATE Idioma SET Nombre = @Nombre, EsDefault = @EsDefault WHERE ID = @ID";
        protected override string SqlEliminar => @"DELETE FROM Idioma WHERE ID = @ID";
        protected override string SqlListar => @"SELECT ID, Nombre, EsDefault FROM Idioma ORDER BY Nombre";
        protected override string SqlObtenerPorId => @"SELECT ID, Nombre, EsDefault FROM Idioma WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Idioma entidad)
        {
            if (cmd.CommandText.Contains("@ID"))
            {
                cmd.Parameters.AddWithValue("@ID", entidad.ID);
            }
            cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@EsDefault", entidad.EsDefault);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Idioma Mapear(SqlDataReader lector)
        {
            return new Idioma(
                Convert.ToInt32(lector["ID"]),
                lector["Nombre"].ToString(),
                Convert.ToBoolean(lector["EsDefault"])
            );
        }

        public override Idioma ObtenerPorEntidad(Idioma entidad)
        {
            return ObtenerPorId(entidad.ID);
        }

        public Idioma ObtenerDefault()
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = "SELECT TOP 1 ID, Nombre, EsDefault FROM Idioma WHERE EsDefault = 1";
                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return Mapear(reader);
                }
            }
            return null;
        }

        public Dictionary<string, string> ObtenerTraduccionesPorIdioma(int idIdioma)
        {
            var traducciones = new Dictionary<string, string>();
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = @"
            -- 1. Buscamos el ID del idioma por defecto (Base / Español)
            DECLARE @IdDefault INT = (SELECT TOP 1 ID FROM Idioma WHERE EsDefault = 1);

            -- 2. Traemos la traducción seleccionada; si está vacía o es NULL, caemos al idioma base
            SELECT 
                E.Clave,
                COALESCE(
                    NULLIF(T_Elegido.Texto, ''),   -- 1º Opción: El texto en Francés
                    NULLIF(T_Default.Texto, ''),   -- 2º Opción: Fallback al Español (Base)
                    '[' + E.Clave + ']'            -- 3º Opción: Clave cruda si nadie la tradujo jamás
                ) AS Texto
            FROM Etiqueta E
            LEFT JOIN Traduccion T_Elegido 
                ON E.ID = T_Elegido.IdEtiqueta AND T_Elegido.IdIdioma = @IdIdioma
            LEFT JOIN Traduccion T_Default 
                ON E.ID = T_Default.IdEtiqueta AND T_Default.IdIdioma = @IdDefault";

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

        
    }
}
