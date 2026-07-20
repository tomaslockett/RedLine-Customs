using RedLine.Be.Interfaces;
using RedLine.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public abstract class AbstractBLL<TKey, entidad> : IRepositorioBasico<TKey, entidad>, IGestorIntegridad
    {
        protected IRepositorioBasico<TKey, entidad> _repositorio;

        public AbstractBLL(IRepositorioBasico<TKey, entidad> repositorio)
        {
            _repositorio = repositorio;
        }

        public virtual void Eliminar(TKey id)
        {
            _repositorio.Eliminar(id);
        }

        public virtual void Insertar(entidad entidad)
        {
            _repositorio.Insertar(entidad);
        }

        public virtual List<entidad> Listar()
        {
            return _repositorio.Listar();
        }

        public virtual void Modificar(entidad entidad)
        {
            _repositorio.Modificar(entidad);
        }

        public virtual entidad ObtenerPorId(TKey id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public virtual entidad ObtenerPorEntidad(entidad entidad)
        {
            return _repositorio.ObtenerPorEntidad(entidad);
        }

        #region Digitos Verificadores


        public virtual void RecalcularIntegridad()
        {
            // Verificamos si el repositorio actual implementa la interfaz de seguridad
            if (_repositorio is IGestorIntegridad repoSeguro)
            {
                repoSeguro.RecalcularIntegridad();
            }
        }

        // Calcula los códigos actuales de la tabla para comparar contra los guardados.
        public virtual ReporteIntegridad CalcularIntegridadActual()
        {
            if (_repositorio is IGestorIntegridad repoSeguro)
            {
                return repoSeguro.CalcularIntegridadActual();
            }

            return new ReporteIntegridad
            {
                DVH_Actual = "N/A",
                DVV_Actual = "N/A"
            };
        }


        // Retorna el nombre de la tabla que este BLL gestiona.
        public virtual string ObtenerNombreTabla()
        {
            if (_repositorio is IGestorIntegridad repoSeguro)
            {
                return repoSeguro.ObtenerNombreTabla();
            }
            return "Tabla_No_Protegida";
        }

        #endregion
    }
}
