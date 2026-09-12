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
    public class DAL_Etiqueta : AbstractDAL<int, Etiqueta>
    {
        protected override string NombreTabla => "Etiqueta";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar =>
            @"INSERT INTO Etiqueta (Clave, Pagina, Descripcion, EsMensajeAlerta) 
              VALUES (@Clave, @Pagina, @Descripcion, @EsMensajeAlerta)";

        protected override string SqlModificar =>
            @"UPDATE Etiqueta 
              SET Clave = @Clave, Pagina = @Pagina, Descripcion = @Descripcion, EsMensajeAlerta = @EsMensajeAlerta 
              WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Etiqueta WHERE ID = @ID";

        protected override string SqlListar => "SELECT * FROM Etiqueta ORDER BY Pagina, Clave";

        protected override string SqlObtenerPorId => "SELECT * FROM Etiqueta WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Etiqueta entidad)
        {
            if (cmd.CommandText.Contains("@ID"))
            {
                cmd.Parameters.AddWithValue("@ID", entidad.ID);
            }
            cmd.Parameters.AddWithValue("@Clave", entidad.Clave);
            cmd.Parameters.AddWithValue("@Pagina", entidad.Pagina);
            cmd.Parameters.AddWithValue("@Descripcion", (object)entidad.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EsMensajeAlerta", entidad.EsMensajeAlerta);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Etiqueta Mapear(SqlDataReader lector)
        {
            return new Etiqueta
            {
                ID = Convert.ToInt32(lector["ID"]),
                Clave = lector["Clave"].ToString(),
                Pagina = lector["Pagina"].ToString(),
                Descripcion = lector["Descripcion"] != DBNull.Value ? lector["Descripcion"].ToString() : null,
                EsMensajeAlerta = Convert.ToBoolean(lector["EsMensajeAlerta"])
            };
        }

        public override Etiqueta ObtenerPorEntidad(Etiqueta entidad)
        {
            return ObtenerPorId(entidad.ID);
        }

        public List<Etiqueta> ListarPorPagina(string pagina)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string sql = "SELECT * FROM Etiqueta WHERE Pagina = @Pagina ORDER BY Clave";
                return con.EjecutarLectura(sql, Mapear, cmd => cmd.Parameters.AddWithValue("@Pagina", pagina));
            }
        }
    }
}
