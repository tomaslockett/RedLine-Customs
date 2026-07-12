using RedLine.Dal.Mappers;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.Bll
{
    public class BLL_Permisos : AbstractBLL<int, ComponentePermiso>
    {
        private BLL_Evento _bllEvento;
        public BLL_Permisos() : base(new DAL_Permisos()) 
        {
            _bllEvento = new BLL_Evento();
        }

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

        public override void Insertar(ComponentePermiso entidad)
        {
            base.Insertar(entidad);
            RegistrarEventoBitacora($"Se creó un nuevo componente de permiso: {entidad.Nombre}", 1);
        }

        public override void Modificar(ComponentePermiso entidad)
        {
            base.Modificar(entidad);
            RegistrarEventoBitacora($"Se modificó el componente de permiso: {entidad.Nombre}", 2);
        }

        public override void Eliminar(int id)
        {
            var permisoAEliminar = base.ObtenerPorId(id);
            string detalle = permisoAEliminar != null ? permisoAEliminar.Nombre : $"ID {id}";

            base.Eliminar(id);
            RegistrarEventoBitacora($"Se eliminó el componente de permiso: {detalle}", 3);
        }
        private void RegistrarEventoBitacora(string mensaje, int criticidad)
        {
            try
            {
                string usuario = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
                _bllEvento.Registrar(usuario, ModulosEventos.Seguridad, mensaje, criticidad);
            }
            catch { }
        }
    }
}
