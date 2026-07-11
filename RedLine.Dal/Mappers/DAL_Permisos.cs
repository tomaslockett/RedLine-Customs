using RedLine.Dal.ORM;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.Mappers
{
    public class DAL_Permisos : AbstractDAL<int, ComponentePermiso>
    {
        protected override string NombreTabla => "Componentes"; 
        protected override bool RequiereDigitoVerificador => true;
        protected override string SqlInsertar => "INSERT INTO Componentes (Id, Nombre, TipoComponente) VALUES (@Id, @Nombre, 'Permiso')";
        protected override string SqlModificar => "UPDATE Componentes SET Nombre = @Nombre WHERE Id = @Id AND TipoComponente = 'Permiso'";
        protected override string SqlEliminar => "DELETE FROM Componentes WHERE Id = @Id AND TipoComponente = 'Permiso'";

        protected override string SqlListar => "SELECT Id, Nombre FROM Componentes WHERE TipoComponente = 'Permiso'";
        protected override string SqlObtenerPorId => "SELECT Id, Nombre FROM Componentes WHERE Id = @Id AND TipoComponente = 'Permiso'";

        protected override void ConfigurarParametros(SqlCommand cmd, ComponentePermiso entidad)
        {
            cmd.AgregarParametro("@Nombre", entidad.Nombre);

            if (cmd.CommandText.Contains("INSERT") || cmd.CommandText.Contains("UPDATE"))
            {
                cmd.AgregarParametro("@Id", entidad.Id);
            }
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.AgregarParametro("@Id", id);
        }

        protected override ComponentePermiso Mapear(SqlDataReader lector)
        {
            int id = lector.Obtener<int>("Id");
            string nombre = lector.ObtenerCadenaSegura("Nombre");
            return new Permiso(id, nombre);
        }

        public override ComponentePermiso ObtenerPorEntidad(ComponentePermiso entidad)
        {
            return ObtenerPorId(entidad.Id);
        }
    }
}
