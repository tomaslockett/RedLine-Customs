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
    public class DAL_Familia : AbstractDAL<int, Familia>
    {
        // ⚠️  Según tu DbContext, Familias y Permisos comparten tabla.
        protected override string NombreTabla => "Componentes";

        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar => "INSERT INTO Componentes (Id, Nombre, TipoComponente) VALUES (@Id, @Nombre, 'Familia')";
        protected override string SqlModificar => "UPDATE Componentes SET Nombre = @Nombre WHERE Id = @Id AND TipoComponente = 'Familia'";
        protected override string SqlEliminar => "DELETE FROM Componentes WHERE Id = @Id AND TipoComponente = 'Familia'";

        protected override string SqlListar => "SELECT Id, Nombre FROM Componentes WHERE TipoComponente = 'Familia'";
        protected override string SqlObtenerPorId => "SELECT Id, Nombre FROM Componentes WHERE Id = @Id AND TipoComponente = 'Familia'";

        protected override void ConfigurarParametros(SqlCommand cmd, Familia entidad)
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

        protected override Familia Mapear(SqlDataReader lector)
        {
            return new Familia(
                lector.Obtener<int>("Id"),
                lector.ObtenerCadenaSegura("Nombre")
            );
        }

        public override Familia ObtenerPorEntidad(Familia entidad)
        {
            return ObtenerPorId(entidad.Id);
        }

        #region Gestión de Jerarquía (Patrón Composite)

        /// <summary>
        /// Guarda la relación Padre-Hijo en la tabla intermedia Permisos_Jerarquia
        /// </summary>
        public void GuardarRelacionPadreHijo(int idPadre, int idHijo)
        {
            string query = "INSERT INTO Permisos_Jerarquia (IdPadre, IdHijo) VALUES (@IdPadre, @IdHijo)";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, con, tra))
                        {
                            cmd.AgregarParametro("@IdPadre", idPadre);
                            cmd.AgregarParametro("@IdHijo", idHijo);
                            cmd.ExecuteNonQuery();
                        }

                        tra.Commit();
                    }
                    catch (Exception)
                    {
                        tra.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Elimina todas las relaciones de una Familia (útil antes de actualizar su árbol de permisos)
        /// </summary>
        public void EliminarRelacionesPorFamilia(int idPadre)
        {
            string query = "DELETE FROM Permisos_Jerarquia WHERE IdPadre = @IdPadre";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, con, tra))
                        {
                            cmd.AgregarParametro("@IdPadre", idPadre);
                            cmd.ExecuteNonQuery();
                        }
                        tra.Commit();
                    }
                    catch (Exception)
                    {
                        tra.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion
    }
}

