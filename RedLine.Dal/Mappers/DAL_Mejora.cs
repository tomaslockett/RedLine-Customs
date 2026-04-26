using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_Mejora : AbstractDAL<int, Mejora>
    {
        protected override string NombreTabla => "Mejora";
        protected override bool RequiereDigitoVerificador => false;

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
            cmd.Parameters.AddWithValue("@Categoria", entidad.Categoria);
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

        protected override Mejora Mapear(SqlDataReader lector)
        {
            string cat = lector["Categoria"].ToString();
            Mejora m;

            switch (cat)
            {
                case "Aleron": m = new Aleron(); break;
                case "Llantas": m = new Llantas(); break;
                case "Suspension": m = new Suspension(); break;
                case "Pintura": m = new Pintura(); break;
                case "Kit": m = new KitCarroceria(); break;
                default: throw new Exception("Categoría de mejora desconocida");
            }

            m.ID = Convert.ToInt32(lector["ID"]);
            m.Nombre = lector["Nombre"].ToString();
            m.Precio = Convert.ToDecimal(lector["Precio"]);
            m.Stock = Convert.ToInt32(lector["Stock"]);
            m.Categoria = cat;
            return m;
        }

        public override Mejora ObtenerPorEntidad(Mejora entidad) => ObtenerPorId(entidad.ID);
    }
}