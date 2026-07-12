using RedLine.Be.Entidades;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal
{
    public class DAL_DigitoVerificador : AbstractDAL<string, DigitoVerificador>
    {
        protected override string NombreTabla => "DigitoVerificador";

        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar =>
            "INSERT INTO DigitoVerificador (NombreTabla, DVH, DVV) VALUES (@Id, @DVH, @DVV)";

        protected override string SqlModificar =>
            "UPDATE DigitoVerificador SET DVH = @DVH, DVV = @DVV WHERE NombreTabla = @Id";

        protected override string SqlEliminar =>
            "DELETE FROM DigitoVerificador WHERE NombreTabla = @Id";

        protected override string SqlListar =>
            "SELECT NombreTabla, DVH, DVV FROM DigitoVerificador";

        protected override string SqlObtenerPorId =>
            "SELECT NombreTabla, DVH, DVV FROM DigitoVerificador WHERE NombreTabla = @Id";

        protected override void ConfigurarParametros(SqlCommand cmd, DigitoVerificador entidad)
        {
            cmd.Parameters.AddWithValue("@Id", entidad.NombreTabla);
            cmd.Parameters.AddWithValue("@DVH", entidad.DVH);
            cmd.Parameters.AddWithValue("@DVV", entidad.DVV);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, string id)
        {
            cmd.Parameters.AddWithValue("@Id", id);
        }

        protected override DigitoVerificador Mapear(SqlDataReader lector)
        {
            return new DigitoVerificador
            {
                NombreTabla = lector["NombreTabla"].ToString(),
                DVH = lector["DVH"].ToString(),
                DVV = lector["DVV"].ToString()
            };
        }

        public override DigitoVerificador ObtenerPorEntidad(DigitoVerificador entidad)
        {
            return ObtenerPorId(entidad.NombreTabla);
        }
    }
}
