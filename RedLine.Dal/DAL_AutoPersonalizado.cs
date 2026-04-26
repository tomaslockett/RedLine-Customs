using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_AutoPersonalizado : AbstractDAL<int, AutoPersonalizado>
    {
        protected override string NombreTabla => "AutoPersonalizado";
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar => "INSERT INTO AutoPersonalizado (DNI_Cliente, ID_AutoBase) OUTPUT INSERTED.ID VALUES (@DNI, @IDBase)";
        protected override string SqlModificar => "UPDATE AutoPersonalizado SET DNI_Cliente = @DNI, ID_AutoBase = @IDBase WHERE ID = @ID";
        protected override string SqlEliminar => "DELETE FROM AutoPersonalizado WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM AutoPersonalizado";
        protected override string SqlObtenerPorId => "SELECT * FROM AutoPersonalizado WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, AutoPersonalizado entidad)
        {
            if (entidad.ID > 0) cmd.Parameters.AddWithValue("@ID", entidad.ID);
            cmd.Parameters.AddWithValue("@DNI", entidad.DNI_Cliente);
            cmd.Parameters.AddWithValue("@IDBase", entidad.AuBase.ID);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        public void GuardarRelacionMejoras(int idAuto, List<Mejora> mejoras, SqlConnection con, SqlTransaction tra)
        {
            string query = "INSERT INTO Auto_Mejora (ID_Auto, ID_Mejora) VALUES (@idAuto, @idMejora)";
            foreach (var m in mejoras)
            {
                using (var cmd = new SqlCommand(query, con, tra))
                {
                    cmd.Parameters.AddWithValue("@idAuto", idAuto);
                    cmd.Parameters.AddWithValue("@idMejora", m.ID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected override AutoPersonalizado Mapear(SqlDataReader lector)
        {
            return new AutoPersonalizado
            {
                ID = Convert.ToInt32(lector["ID"]),
                DNI_Cliente = lector["DNI_Cliente"].ToString()
            };
        }

        public override AutoPersonalizado ObtenerPorEntidad(AutoPersonalizado entidad) => ObtenerPorId(entidad.ID);
    }
}