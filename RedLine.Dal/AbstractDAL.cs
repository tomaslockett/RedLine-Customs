using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedLine.Be.Interfaces;

namespace RedLine.Dal
{
    public abstract class AbstractDAL<TKey, entidad> : IRepositorioBasico<TKey, entidad>
    {
        public abstract void Eliminar(TKey id);
        public abstract void Insertar(entidad entidad);
        public abstract List<entidad> Listar();
        public abstract void Modificar(entidad entidad);
        public abstract entidad ObtenerPorId(TKey id);
        public abstract entidad ObtenerPorEntidad(entidad entidad);
    }
}
