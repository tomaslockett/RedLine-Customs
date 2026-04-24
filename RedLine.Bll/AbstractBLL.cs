using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedLine.Be.Interfaces;

namespace RedLine.Bll
{
    public abstract class AbstractBLL<TKey, entidad> : IRepositorioBasico<TKey, entidad>
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
    }
}
