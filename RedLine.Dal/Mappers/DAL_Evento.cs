using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.Mappers
{
    public class DAL_Evento : AbstractDAL<int, Evento>
    {
        protected override string NombreTabla => "Bitacora";
        protected override bool RequiereDigitoVerificador => false; 

        protected override string SqlInsertar =>
            "INSERT INTO Bitacora (Usuario, Fecha, Modulo, Actividad, Criticidad) VALUES (@Usu, @Fec, @Mod, @Act, @Cri)";

        protected override string SqlListar => "SELECT * FROM Bitacora ORDER BY Fecha DESC";
        protected override string SqlObtenerPorId => "SELECT * FROM Bitacora WHERE ID = @ID";

        protected override string SqlModificar => "";
        protected override string SqlEliminar => "";

        protected override void ConfigurarParametros(SqlCommand cmd, Evento entidad)
        {
            cmd.Parameters.AddWithValue("@Usu", entidad.Usuario);
            cmd.Parameters.AddWithValue("@Fec", entidad.Fecha);
            cmd.Parameters.AddWithValue("@Mod", entidad.Modulo);
            cmd.Parameters.AddWithValue("@Act", entidad.Actividad);
            cmd.Parameters.AddWithValue("@Cri", entidad.Criticidad);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Evento Mapear(SqlDataReader lector)
        {
            return new Evento
            {
                ID = Convert.ToInt32(lector["ID"]),
                Usuario = lector["Usuario"].ToString(),
                Fecha = Convert.ToDateTime(lector["Fecha"]),
                Modulo = lector["Modulo"].ToString(),
                Actividad = lector["Actividad"].ToString(),
                Criticidad = Convert.ToInt32(lector["Criticidad"])
            };
        }

        public override Evento ObtenerPorEntidad(Evento entidad) => null;
    }
}
