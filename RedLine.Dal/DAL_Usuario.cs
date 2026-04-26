using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedLine.Be;

namespace RedLine.Dal
{
    public class DAL_Usuario 
    {
        private string cx = "cadena coming soon";

        public void AltaUsuario(Usuario usuario)
        {
            string query = @"INSERT INTO Usuario (Username, Contraseña, Rol, Intentos, Bloqueado, Activo, UltimoIntento) 
                             VALUES (@Username, @Contraseña, @Rol, 0, 0, 0, @UltimoIntento)";

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

        public void ActivarUsuario(Usuario usuario)
        {
            string consulta = "UPDATE Usuario SET Activo = 1 WHERE ID = @ID";
            using (SqlConnection conexion = new SqlConnection(cx))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", usuario.ID);

                    comando.ExecuteNonQuery();
                }
            }
        }

        public void DesactivarUsuario(Usuario usuario)
        {
            string consulta = "UPDATE Usuario SET Activo = 0 WHERE ID = @ID";
            using (SqlConnection conexion = new SqlConnection(cx))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", usuario.ID);

                    comando.ExecuteNonQuery();
                }
            }
        }

        public void ActualizarUsuario(Usuario usuario)
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
                cmd.Parameters.AddWithValue("@Activo", usuario.Activo);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Usuario> ObtenerUsuarios()
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
                            Convert.ToBoolean(reader["Activo"]),
                            Convert.ToDateTime(reader["UltimoIntento"])
                        ));
                    }
                }
            }
            return lista;
        }

        public void DesbloquearUsuario(Usuario usuario)
        {
            string query = "UPDATE Usuario SET Bloqueado = 0 WHERE ID = @id";
            using (SqlConnection conexion = new SqlConnection(cx))
            {
                conexion.Open();
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@ID", usuario.ID);

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
                            Convert.ToBoolean(reader["Activo"]),
                            Convert.ToDateTime(reader["UltimoIntento"])
                        );
                    }
                }
            }
            return null;
        }
    }
}
