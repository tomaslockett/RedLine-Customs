using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_AutoBase : AbstractDAL<int, AutoBase>
    {
        protected override string NombreTabla => "AutoBase";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar => "INSERT INTO AutoBase (Marca, Modelo, PrecioBase) VALUES (@Marca, @Modelo, @PrecioBase)";
        protected override string SqlModificar => "UPDATE AutoBase SET Marca = @Marca, Modelo = @Modelo, PrecioBase = @PrecioBase WHERE ID = @ID";
        protected override string SqlEliminar => "DELETE FROM AutoBase WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM AutoBase";
        protected override string SqlObtenerPorId => "SELECT * FROM AutoBase WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, AutoBase entidad)
        {
            if (entidad.ID > 0) cmd.Parameters.AddWithValue("@ID", entidad.ID);
            cmd.Parameters.AddWithValue("@Marca", entidad.Marca);
            cmd.Parameters.AddWithValue("@Modelo", entidad.Modelo);
            cmd.Parameters.AddWithValue("@PrecioBase", entidad.PrecioBase);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override AutoBase Mapear(SqlDataReader lector)
        {
            return new AutoBase(
                Convert.ToInt32(lector["ID"]),
                lector["Marca"].ToString(),
                lector["Modelo"].ToString(),
                Convert.ToDecimal(lector["PrecioBase"])
            );
        }

        public override AutoBase ObtenerPorEntidad(AutoBase entidad) => ObtenerPorId(entidad.ID);
    }
}