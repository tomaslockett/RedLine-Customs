using RedLine.Be.Interfaces;
using RedLine.Dal.ORM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal
{
    // ⚠️ ATENCIÓN REQUISITO DE BD: 
    // Para que este motor funcione, DEBE existir en la base de datos una tabla 
    // llamada exactamente 'DigitoVerificador' con las columnas: NombreTabla (PK), DVH, DVV.
    public abstract class AbstractDAL<TKey, entidad> : IRepositorioBasico<TKey, entidad>
    {
        protected string cx = "cadena coming soon";

        #region Configuracion
        protected abstract string NombreTabla { get; }
        protected abstract bool RequiereDigitoVerificador { get; }
        protected abstract string SqlInsertar { get; }
        protected abstract string SqlModificar { get; }
        protected abstract string SqlEliminar { get; }
        protected abstract string SqlListar { get; }
        protected abstract string SqlObtenerPorId { get; }

        protected abstract void ConfigurarParametros(SqlCommand cmd, entidad entidad);
        protected abstract void ConfigurarParametrosId(SqlCommand cmd, TKey id);
        protected abstract entidad Mapear(SqlDataReader lector);

        #endregion

        #region Operaciones Base de datos

        public virtual void Insertar(entidad entidad)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(SqlInsertar, cmd => ConfigurarParametros(cmd, entidad));
                if (RequiereDigitoVerificador) RedLine.Servicios.DigitoVerificador.Recalcular(con, NombreTabla);
            }
        }

        public virtual void Modificar(entidad entidad)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(SqlModificar, cmd => ConfigurarParametros(cmd, entidad));
                if (RequiereDigitoVerificador) RedLine.Servicios.DigitoVerificador.Recalcular(con, NombreTabla);
            }
        }

        public virtual void Eliminar(TKey id)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(SqlEliminar, cmd => ConfigurarParametrosId(cmd, id));
                if (RequiereDigitoVerificador) RedLine.Servicios.DigitoVerificador.Recalcular(con, NombreTabla);
            }
        }

        public virtual List<entidad> Listar()
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                return con.EjecutarLectura(SqlListar, Mapear);
            }
        }

        public virtual entidad ObtenerPorId(TKey id)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                var lista = con.EjecutarLectura(SqlObtenerPorId, Mapear, cmd => ConfigurarParametrosId(cmd, id));
                return lista.FirstOrDefault();
            }
        }

        public abstract entidad ObtenerPorEntidad(entidad entidad);

        #endregion
    }
}
