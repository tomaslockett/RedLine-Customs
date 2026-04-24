using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Be.Interfaces
{
    public interface IRepositorioBasico<TKey, entidad>
    {
        //ALTA
        void Insertar(entidad entidad);

        //BAJA
        void Eliminar(TKey id);

        //MODIFICACIÓN
        void Modificar(entidad entidad);

        //LECTURAS
        List<entidad> Listar();
        entidad ObtenerPorId(TKey id);
        entidad ObtenerPorEntidad(entidad entidad);
    }
}
