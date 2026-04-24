using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_Usuario : AbstractDAL<int, Usuario>
    {
        private string cx = "cadena coming soon";

        public override void Insertar(Usuario usuario)
        {
            string query = @"INSERT INTO Usuario (Username, Contraseña, Rol, Intentos, Bloqueado, UltimoIntento) 
                             VALUES (@Username, @Contraseña, @Rol, 0, 0, @UltimoIntento)";

            using (SqlConnection con = new SqlConnection(cx))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", usuario.Username);
                cmd.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                cmd.Parameters.AddWithValue("@UltimoIntento", DateTime.Now);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public override void Eliminar(int id)
        {
            string query = "DELETE FROM Usuario WHERE ID = @ID";

            using (SqlConnection con = new SqlConnection(cx))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public override void Modificar(Usuario usuario)
        {
            string query = @"UPDATE Usuario
                             SET Username = @Username, Rol = @Rol, Bloqueado = @Bloqueado 
                             WHERE ID = @ID";

            using (SqlConnection con = new SqlConnection(cx))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", usuario.Username);
                cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                cmd.Parameters.AddWithValue("@Bloqueado", usuario.Bloqueado);
                cmd.Parameters.AddWithValue("@ID", usuario.ID);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public override List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();
            string query = "SELECT * FROM Usuario";

            using (SqlConnection con = new SqlConnection(cx))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Usuario(
                            Convert.ToInt32(reader["ID"]),
                            reader["Username"].ToString(),
                            reader["Contraseña"].ToString(),
                            reader["Rol"].ToString(),
                            Convert.ToInt32(reader["Intentos"]),
                            Convert.ToBoolean(reader["Bloqueado"]),
                            Convert.ToDateTime(reader["UltimoIntento"])
                        ));
                    }
                }
            }
            return lista;
        }

        public override Usuario ObtenerPorId(int id)
        {
            string query = "SELECT * FROM Usuario WHERE ID = @ID";
            using (SqlConnection con = new SqlConnection(cx))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario(
                            Convert.ToInt32(reader["ID"]),
                            reader["Username"].ToString(),
                            reader["Contraseña"].ToString(),
                            reader["Rol"].ToString(),
                            Convert.ToInt32(reader["Intentos"]),
                            Convert.ToBoolean(reader["Bloqueado"]),
                            Convert.ToDateTime(reader["UltimoIntento"])
                        );
                    }
                }
            }
            return null;
        }

        public override Usuario ObtenerPorEntidad(Usuario entidad)
        {
            if (!string.IsNullOrEmpty(entidad.Username))
            {
                return ObtenerPorUsername(entidad.Username);
            }
            return null;
        }

        // MÉTODOS EXTRAS ESPECÍFICOS DE USUARIO 
      

        public void DesbloquearUsuario(Usuario usuario)
        {
            string query = "UPDATE Usuario SET Bloqueado = 0 WHERE ID = @id";
            using (SqlConnection conexion = new SqlConnection(cx))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand(query, conexion)) 
                {
                    comando.Parameters.AddWithValue("@id", usuario.ID); 
                    comando.ExecuteNonQuery();
                }
            }
        }

        public Usuario ObtenerPorUsername(string username)
        {
            string query = "SELECT * FROM Usuario WHERE Username = @Username";
            using (SqlConnection con = new SqlConnection(cx))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Username", username);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario(
                            Convert.ToInt32(reader["ID"]),
                            reader["Username"].ToString(),
                            reader["Contraseña"].ToString(),
                            reader["Rol"].ToString(),
                            Convert.ToInt32(reader["Intentos"]),
                            Convert.ToBoolean(reader["Bloqueado"]),
                            Convert.ToDateTime(reader["UltimoIntento"])
                        );
                    }
                }
            }
            return null;
        }
    }
}
