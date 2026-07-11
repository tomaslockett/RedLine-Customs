using RedLine.Dal.Mappers;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.Bll
{
    public class BLL_Permisos : AbstractBLL<int, ComponentePermiso>
    {
        public BLL_Permisos() : base(new DAL_Permisos()) { }

        public void GuardarPermiso(ComponentePermiso permiso)
        {

            if (string.IsNullOrEmpty(permiso.Nombre))
            {
                throw new Exception("El nombre del permiso es obligatorio.");
            }

            _repositorio.Insertar(permiso);
        }
        public List<Familia> ListarFamilias()
        {
            return _repositorio.Listar().OfType<Familia>().ToList();
        }
    }
}
