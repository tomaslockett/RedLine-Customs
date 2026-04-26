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
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar =>
            @"INSERT INTO Usuario (Username, Contraseña, Rol, Intentos, Bloqueado, Activo, UltimoIntento) 
              VALUES (@Username, @Contraseña, @Rol, 0, 0, 1, @UltimoIntento)";

        protected override string SqlModificar =>
            @"UPDATE Usuario SET Username = @Username, Rol = @Rol, Bloqueado = @Bloqueado, 
              Activo = @Activo, Intentos = @Intentos, UltimoIntento = @UltimoIntento WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Usuario WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM Usuario";
        protected override string SqlObtenerPorId => "SELECT * FROM Usuario WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Usuario entidad)
        {
            if (entidad.ID > 0) cmd.Parameters.AddWithValue("@ID", entidad.ID);
            cmd.Parameters.AddWithValue("@Username", entidad.Username);
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
                lector["Username"].ToString(),
                lector["Contraseña"].ToString(),
                lector["Rol"].ToString(),
                Convert.ToInt32(lector["Intentos"]),
                Convert.ToBoolean(lector["Bloqueado"]),
                Convert.ToBoolean(lector["Activo"]),
                Convert.ToDateTime(lector["UltimoIntento"])
            );
        }

        public Usuario ObtenerPorUsername(string username)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                string query = "SELECT * FROM Usuario WHERE Username = @un";
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@un", username);
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
            return ObtenerPorUsername(entidad.Username);
        }
    }
}