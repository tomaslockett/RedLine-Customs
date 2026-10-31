using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_Mejora : AbstractDAL<int, Mejora>
    {
        protected override string NombreTabla => "Mejora";
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar =>
            @"INSERT INTO Mejora (Nombre, Precio, Stock, Categoria) 
              VALUES (@Nombre, @Precio, @Stock, @Categoria)";

        protected override string SqlModificar =>
            @"UPDATE Mejora SET Nombre = @Nombre, Precio = @Precio, 
              Stock = @Stock, Categoria = @Categoria WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Mejora WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM Mejora";
        protected override string SqlObtenerPorId => "SELECT * FROM Mejora WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Mejora entidad)
        {
            if (entidad.ID > 0) cmd.Parameters.AddWithValue("@ID", entidad.ID);
            cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@Precio", entidad.Precio);
            cmd.Parameters.AddWithValue("@Stock", entidad.Stock);
            cmd.Parameters.AddWithValue("@Categoria", (object)entidad.Categoria ?? DBNull.Value);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        public void RestarStock(int idMejora, int cantidad, SqlConnection con, SqlTransaction tra)
        {
            string query = "UPDATE Mejora SET Stock = Stock - @cant WHERE ID = @id AND Stock >= @cant";
            using (SqlCommand cmd = new SqlCommand(query, con, tra))
            {
                cmd.Parameters.AddWithValue("@id", idMejora);
                cmd.Parameters.AddWithValue("@cant", cantidad);

                int filasAfectadas = cmd.ExecuteNonQuery();
                if (filasAfectadas == 0)
                    throw new Exception($"No hay stock disponible para la mejora con ID {idMejora}");
            }
        }


        public Mejora ObtenerPorClave(string clave)
        {
            Mejora mejora = null;
            string query = "SELECT * FROM Mejora WHERE LOWER(Nombre) LIKE @clave";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@clave", "%" + clave.ToLower() + "%");
                    using (var lector = cmd.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            mejora = Mapear(lector);
                        }
                    }
                }
            }
            return mejora;
        }

        protected override Mejora Mapear(SqlDataReader lector)
        {
            return new Mejora
            {
                ID = Convert.ToInt32(lector["ID"]),
                Nombre = lector["Nombre"].ToString(),
                Precio = Convert.ToDecimal(lector["Precio"]),
                Stock = Convert.ToInt32(lector["Stock"]),
                Categoria = lector["Categoria"] != DBNull.Value ? lector["Categoria"].ToString() : string.Empty
            };
        }

        public override Mejora ObtenerPorEntidad(Mejora entidad) => ObtenerPorId(entidad.ID);
    }
}