using RedLine.Be.Entidades;
using RedLine.Dal.ORM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.Mappers
{
    public class DAL_Traduccion : AbstractDAL<int, Traduccion>
    {
        protected override string NombreTabla => "Traduccion";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar =>
            @"INSERT INTO Traduccion (IdIdioma, IdEtiqueta, Texto) 
              VALUES (@IdIdioma, @IdEtiqueta, @Texto)";

        protected override string SqlModificar =>
            @"UPDATE Traduccion 
              SET IdIdioma = @IdIdioma, IdEtiqueta = @IdEtiqueta, Texto = @Texto 
              WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Traduccion WHERE ID = @ID";

        protected override string SqlListar => "SELECT * FROM Traduccion";

        protected override string SqlObtenerPorId => "SELECT * FROM Traduccion WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Traduccion entidad)
        {
            if (cmd.CommandText.Contains("@ID"))
            {
                cmd.Parameters.AddWithValue("@ID", entidad.ID);
            }
            cmd.Parameters.AddWithValue("@IdIdioma", entidad.IdIdioma);
            cmd.Parameters.AddWithValue("@IdEtiqueta", entidad.IdEtiqueta);
            cmd.Parameters.AddWithValue("@Texto", entidad.Texto ?? string.Empty);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Traduccion Mapear(SqlDataReader lector)
        {
            return new Traduccion
            {
                ID = Convert.ToInt32(lector["ID"]),
                IdIdioma = Convert.ToInt32(lector["IdIdioma"]),
                IdEtiqueta = Convert.ToInt32(lector["IdEtiqueta"]),
                Texto = lector["Texto"].ToString()
            };
        }

        public override Traduccion ObtenerPorEntidad(Traduccion entidad)
        {
            return ObtenerPorId(entidad.ID);
        }


        private Traduccion MapearDetallado(SqlDataReader lector)
        {
            return new Traduccion
            {
                ID = Convert.ToInt32(lector["IdTraduccion"]),
                IdIdioma = Convert.ToInt32(lector["IdIdioma"]),
                IdEtiqueta = Convert.ToInt32(lector["IdEtiqueta"]),
                Texto = lector["Texto"].ToString(),
                Etiqueta = new Etiqueta
                {
                    ID = Convert.ToInt32(lector["IdEtiqueta"]),
                    Clave = lector["Clave"].ToString(),
                    Pagina = lector["Pagina"].ToString(),
                    Descripcion = lector["Descripcion"] != DBNull.Value ? lector["Descripcion"].ToString() : null,
                    EsMensajeAlerta = Convert.ToBoolean(lector["EsMensajeAlerta"])
                }
            };
        }


        public List<Traduccion> ListarPorIdiomaYPagina(int idIdioma, string pagina = null)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        ISNULL(T.ID, 0) AS IdTraduccion,
                        @IdIdioma AS IdIdioma,
                        ISNULL(T.Texto, '') AS Texto,
                        E.ID AS IdEtiqueta,
                        E.Clave,
                        E.Pagina,
                        E.Descripcion,
                        E.EsMensajeAlerta
                    FROM Etiqueta E
                    LEFT JOIN Traduccion T ON E.ID = T.IdEtiqueta AND T.IdIdioma = @IdIdioma
                    WHERE (@Pagina IS NULL OR E.Pagina = @Pagina)
                    ORDER BY E.Pagina, E.Clave";

                return con.EjecutarLectura(sql, MapearDetallado, cmd =>
                {
                    cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);
                    cmd.Parameters.AddWithValue("@Pagina", (object)pagina ?? DBNull.Value);
                });
            }
        }


        public void GuardarOActualizar(int idIdioma, int idEtiqueta, string texto)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string sql = @"
                    IF EXISTS (SELECT 1 FROM Traduccion WHERE IdIdioma = @IdIdioma AND IdEtiqueta = @IdEtiqueta)
                        UPDATE Traduccion SET Texto = @Texto WHERE IdIdioma = @IdIdioma AND IdEtiqueta = @IdEtiqueta;
                    ELSE
                        INSERT INTO Traduccion (IdIdioma, IdEtiqueta, Texto) VALUES (@IdIdioma, @IdEtiqueta, @Texto);";

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);
                    cmd.Parameters.AddWithValue("@IdEtiqueta", idEtiqueta);
                    cmd.Parameters.AddWithValue("@Texto", texto ?? string.Empty);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
