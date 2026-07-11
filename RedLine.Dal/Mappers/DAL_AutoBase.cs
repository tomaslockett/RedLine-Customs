using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_AutoBase : AbstractDAL<int, AutoBase>
    {
        protected override string NombreTabla => "AutoBase";
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar =>
            @"INSERT INTO AutoBase
            (CodigoVehiculo, Marca, Modelo, Anio, PrecioBase, Stock, Tipo,
            Potencia, VelocidadMaxima, Aceleracion0a100,
            DescripcionGeneral, ImagenBinaria)

            VALUES

            (@CodigoVehiculo, @Marca, @Modelo, @Anio, @PrecioBase, @Stock, @Tipo,
            @Potencia, @VelocidadMaxima, @Aceleracion0a100,
            @DescripcionGeneral, @ImagenBinaria)";
        protected override string SqlModificar =>
            @"UPDATE AutoBase SET
            CodigoVehiculo = @CodigoVehiculo,
            Marca = @Marca,
            Modelo = @Modelo,
            Anio = @Anio,
            PrecioBase = @PrecioBase,
            Stock = @Stock,
            Tipo = @Tipo,
            Potencia = @Potencia,
            VelocidadMaxima = @VelocidadMaxima,
            Aceleracion0a100 = @Aceleracion0a100,
            DescripcionGeneral = @DescripcionGeneral,
            ImagenBinaria = @ImagenBinaria

            WHERE ID = @ID";
        protected override string SqlEliminar => "DELETE FROM AutoBase WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM AutoBase";
        protected override string SqlObtenerPorId => "SELECT * FROM AutoBase WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, AutoBase entidad)
        {
            if (entidad.ID > 0)
                cmd.Parameters.AddWithValue("@ID", entidad.ID);

            cmd.Parameters.AddWithValue("@CodigoVehiculo", entidad.CodigoVehiculo);
            cmd.Parameters.AddWithValue("@Marca", entidad.Marca);
            cmd.Parameters.AddWithValue("@Modelo", entidad.Modelo);
            cmd.Parameters.AddWithValue("@Anio", entidad.Anio);
            cmd.Parameters.AddWithValue("@PrecioBase", entidad.PrecioBase);
            cmd.Parameters.AddWithValue("@Stock", entidad.Stock);
            cmd.Parameters.AddWithValue("@Tipo", entidad.Tipo);
            cmd.Parameters.AddWithValue("@Potencia", entidad.Potencia);
            cmd.Parameters.AddWithValue("@VelocidadMaxima", entidad.VelocidadMaxima);
            cmd.Parameters.AddWithValue("@Aceleracion0a100", entidad.Aceleracion0a100);
            cmd.Parameters.AddWithValue("@DescripcionGeneral", entidad.DescripcionGeneral);
            if (entidad.ImagenBinaria != null)
            {
                cmd.Parameters.Add("@ImagenBinaria", SqlDbType.VarBinary).Value = entidad.ImagenBinaria;
            }
            else
            {
                cmd.Parameters.Add("@ImagenBinaria", SqlDbType.VarBinary).Value = DBNull.Value;
            }
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override AutoBase Mapear(SqlDataReader lector)
        {
            byte[] imagenData = lector["ImagenBinaria"] != DBNull.Value ? (byte[])lector["ImagenBinaria"] : null;

            return new AutoBase(
                Convert.ToInt32(lector["ID"]),
                lector["CodigoVehiculo"].ToString(),

                lector["Marca"].ToString(),
                lector["Modelo"].ToString(),
                Convert.ToInt32(lector["Anio"]),
                Convert.ToDecimal(lector["PrecioBase"]),
                Convert.ToInt32(lector["Stock"]),
                lector["Tipo"].ToString(),

                lector["Potencia"] != DBNull.Value
                    ? Convert.ToInt32(lector["Potencia"])
                    : 0,

                lector["VelocidadMaxima"] != DBNull.Value
                    ? Convert.ToInt32(lector["VelocidadMaxima"])
                    : 0,

                lector["Aceleracion0a100"] != DBNull.Value
                    ? Convert.ToDecimal(lector["Aceleracion0a100"])
                    : 0,

                lector["DescripcionGeneral"].ToString(),
                imagenData
            );
        }

        public override AutoBase ObtenerPorEntidad(AutoBase entidad) => ObtenerPorId(entidad.ID);
    }
}