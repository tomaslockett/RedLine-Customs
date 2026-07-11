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
    public class DAL_Perfil : AbstractDAL<int, Perfil>
    {
        protected override string NombreTabla => "Perfil";

        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar => "INSERT INTO Perfil (Nombre) VALUES (@Nombre)";
        protected override string SqlModificar => "UPDATE Perfil SET Nombre = @Nombre WHERE Id = @Id";
        protected override string SqlEliminar => "DELETE FROM Perfil WHERE Id = @Id";
        protected override string SqlListar => "SELECT Id, Nombre FROM Perfil";
        protected override string SqlObtenerPorId => "SELECT Id, Nombre FROM Perfil WHERE Id = @Id";

        protected override void ConfigurarParametros(SqlCommand cmd, Perfil entidad)
        {
            cmd.AgregarParametro("@Nombre", entidad.Nombre);

            if (cmd.CommandText.Contains("UPDATE"))
            {
                cmd.AgregarParametro("@Id", entidad.Id);
            }
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.AgregarParametro("@Id", id);
        }

        protected override Perfil Mapear(SqlDataReader lector)
        {
            return new Perfil
            {
                Id = lector.Obtener<int>("Id"),
                Nombre = lector.ObtenerCadenaSegura("Nombre")
            };
        }

        public override Perfil ObtenerPorEntidad(Perfil entidad)
        {
            return ObtenerPorId(entidad.Id);
        }

        /// <summary>
        /// Guarda la relación entre un Perfil y un Componente (Familia o Permiso)
        /// en la tabla intermedia Perfil_Componente.
        /// </summary>
        public void GuardarRelacion(int idPerfil, int idComponente)
        {
            string query = "INSERT INTO Perfil_Componente (IdPerfil, IdComponente) VALUES (@IdPerfil, @IdComponente)";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, con, tra))
                        {
                            cmd.AgregarParametro("@IdPerfil", idPerfil);
                            cmd.AgregarParametro("@IdComponente", idComponente);
                            cmd.ExecuteNonQuery();
                        }
                        this.RecalcularMisDigitosVerificadores(con, tra);

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
        /// Elimina la relación entre un Perfil y un Componente específico.
        /// </summary>
        public void EliminarRelacion(int idPerfil, int idComponente)
        {
            string query = "DELETE FROM Perfil_Componente WHERE IdPerfil = @IdPerfil AND IdComponente = @IdComponente";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, con, tra))
                        {
                            cmd.AgregarParametro("@IdPerfil", idPerfil);
                            cmd.AgregarParametro("@IdComponente", idComponente);
                            cmd.ExecuteNonQuery();
                        }

                        this.RecalcularMisDigitosVerificadores(con, tra);

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
        /// Elimina TODAS las relaciones de un Perfil dado. 
        /// Útil antes de sincronizar la lista completa de permisos.
        /// </summary>
        public void EliminarRelacionesPorPerfil(int idPerfil)
        {
            string query = "DELETE FROM Perfil_Componente WHERE IdPerfil = @IdPerfil";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(query, con, tra))
                        {
                            cmd.AgregarParametro("@IdPerfil", idPerfil);
                            cmd.ExecuteNonQuery();
                        }

                        this.RecalcularMisDigitosVerificadores(con, tra);

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

        public List<int> ObtenerIdsComponentesPorPerfil(int idPerfil)
        {
            var ids = new List<int>();
            string query = "SELECT IdComponente FROM Perfil_Componente WHERE IdPerfil = @IdPerfil";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ids.Add(Convert.ToInt32(rdr["IdComponente"]));
                        }
                    }
                }
            }
            return ids;
        }

        /// <summary>
        /// Desarma el Patrón Composite en la base de datos usando una CTE recursiva 
        /// para devolver únicamente las patentes finales (Permisos) de un perfil.
        /// </summary>
        public List<ComponentePermiso> ObtenerPermisosAtomicosDePerfil(int idPerfil)
        {
            var lista = new List<ComponentePermiso>();

            string query = @"
        WITH Jerarquia AS (
            -- Caso base: Componentes (Permisos o Familias) directamente asignados al Perfil
            SELECT c.Id, c.Nombre, c.TipoComponente
            FROM Perfil_Componente pc
            INNER JOIN Componentes c ON pc.IdComponente = c.Id
            WHERE pc.IdPerfil = @IdPerfil

            UNION ALL

            -- Paso recursivo: Busca los Hijos de las Familias encontradas en el paso anterior
            SELECT hijo.Id, hijo.Nombre, hijo.TipoComponente
            FROM Permisos_Jerarquia pj
            INNER JOIN Jerarquia padre ON pj.IdPadre = padre.Id
            INNER JOIN Componentes hijo ON pj.IdHijo = hijo.Id
        )
        -- Retornamos solo los permisos atómicos sin duplicados
        SELECT DISTINCT Id, Nombre 
        FROM Jerarquia 
        WHERE TipoComponente = 'Permiso';";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IdPerfil", idPerfil);
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lista.Add(new Permiso(
                                Convert.ToInt32(rdr["Id"]),
                                rdr["Nombre"].ToString()
                            ));
                        }
                    }
                }
            }
            return lista;
        }

    }
}
