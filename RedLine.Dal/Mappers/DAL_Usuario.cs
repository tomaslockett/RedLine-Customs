using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_Usuario : AbstractDAL<int, Usuario>
    {
        protected override string NombreTabla => "Usuario";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar =>
            @"INSERT INTO Usuario (DNI, Nombre, Apellido, Email, Contraseña, Rol, Intentos, Bloqueado, Activo, UltimoIntento) 
              VALUES (@DNI, @Nombre, @Apellido, @Email, @Contraseña, @Rol, 0, 0, 1, @UltimoIntento)";

        protected override string SqlModificar =>
            @"UPDATE Usuario SET DNI = @DNI, Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Rol = @Rol, Bloqueado = @Bloqueado, 
              Activo = @Activo, Intentos = @Intentos, UltimoIntento = @UltimoIntento WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Usuario WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM Usuario";
        protected override string SqlObtenerPorId => "SELECT * FROM Usuario WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Usuario entidad)
        {
            if (cmd.CommandText.Contains("@ID"))
            {
                cmd.Parameters.AddWithValue("@ID", entidad.ID);
            }
            cmd.Parameters.AddWithValue("@DNI", entidad.DNI);
            cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", entidad.Apellido);
            cmd.Parameters.AddWithValue("@Email", entidad.Email);
            cmd.Parameters.AddWithValue("@Contraseña", entidad.Contraseña);
            cmd.Parameters.AddWithValue("@Rol", entidad.Rol);
            cmd.Parameters.AddWithValue("@Bloqueado", entidad.Bloqueado);
            cmd.Parameters.AddWithValue("@Activo", entidad.Activo);
            cmd.Parameters.AddWithValue("@Intentos", entidad.Intentos);
            cmd.Parameters.AddWithValue("@UltimoIntento", entidad.UltimoIntento);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Usuario Mapear(SqlDataReader lector)
        {
            return new Usuario(
                Convert.ToInt32(lector["ID"]),
                lector["DNI"].ToString(),
                lector["Nombre"].ToString(),
                lector["Apellido"].ToString(),
                lector["Email"].ToString(),
                lector["Contraseña"].ToString(),
                lector["Rol"].ToString(),
                Convert.ToInt32(lector["Intentos"]),
                Convert.ToBoolean(lector["Bloqueado"]),
                Convert.ToBoolean(lector["Activo"]),
                Convert.ToDateTime(lector["UltimoIntento"])
            );
        }

        public Usuario ObtenerPorEmail(string username)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = "SELECT * FROM Usuario WHERE Email = @em";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@em", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return Mapear(reader);
                    }
                }
            }
            return null;
        }

        public override Usuario ObtenerPorEntidad(Usuario entidad)
        {
            return ObtenerPorEmail(entidad.Email);
        }

        public Usuario ObtenerPorDNI(string dni)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = "SELECT * FROM Usuario WHERE DNI = @dni";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return Mapear(reader);
                    }
                }
            }
            return null;
        }
    }
}