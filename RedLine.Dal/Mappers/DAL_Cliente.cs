using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Redline.Be;

namespace RedLine.Dal
{
    public class DAL_Cliente : AbstractDAL<string, Cliente>
    {
        protected override string NombreTabla => "Cliente";
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar =>
            @"INSERT INTO Cliente (DNI, Nombre, Apellido, Email, Telefono, Direccion) 
              VALUES (@DNI, @Nombre, @Apellido, @Email, @Telefono, @Direccion)";

        protected override string SqlModificar =>
            @"UPDATE Cliente SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, 
              Telefono = @Telefono, Direccion = @Direccion WHERE DNI = @DNI";

        protected override string SqlEliminar => "DELETE FROM Cliente WHERE DNI = @DNI";

        protected override string SqlListar => "SELECT * FROM Cliente";

        protected override string SqlObtenerPorId => "SELECT * FROM Cliente WHERE DNI = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Cliente entidad)
        {
            cmd.Parameters.AddWithValue("@DNI", entidad.DNI);
            cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", entidad.Apellido);
            cmd.Parameters.AddWithValue("@Email", entidad.Email);
            cmd.Parameters.AddWithValue("@Telefono", entidad.Telefono);
            cmd.Parameters.AddWithValue("@Direccion", entidad.Direccion);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, string id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Cliente Mapear(SqlDataReader lector)
        {
            return new Cliente
            {
                DNI = lector["DNI"].ToString(),
                Nombre = lector["Nombre"].ToString(),
                Apellido = lector["Apellido"].ToString(),
                Email = lector["Email"].ToString(),
                Telefono = lector["Telefono"].ToString(),
                Direccion = lector["Direccion"].ToString()
            };
        }

        public override Cliente ObtenerPorEntidad(Cliente entidad)
        {
            return ObtenerPorId(entidad.DNI);
        }
    }
}