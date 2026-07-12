using RedLine.Dal.Mappers;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Perfil : AbstractBLL<int, Perfil>
    {
        private BLL_Evento _bllEvento;
        public BLL_Perfil() : base(new DAL_Perfil())
        {
            _bllEvento = new BLL_Evento();
        }

        public void AsignarComponenteAPerfil(int idPerfil, ComponentePermiso componente)
        {
            var perfil = _repositorio.ObtenerPorId(idPerfil);
            if (perfil == null)
                throw new Exception("Perfil no encontrado.");

            perfil.AsignarPermiso(componente);

            DAL_Perfil dalPerfil = (DAL_Perfil)_repositorio;
            dalPerfil.GuardarRelacion(idPerfil, componente.Id);
        }


        /// <summary>
        /// Método para eliminar un componente de un perfil.
        /// </summary>
        public void QuitarComponenteDePerfil(int idPerfil, int idComponente)
        {
            DAL_Perfil dalPerfil = (DAL_Perfil)_repositorio;
            dalPerfil.EliminarRelacion(idPerfil, idComponente);
        }


        public void SincronizarPermisos(int idPerfil, List<int> nuevosIdsPermisos)
        {
            if (idPerfil <= 0)
            {
                throw new ArgumentException("El ID del perfil no es válido.");
            }

            DAL_Perfil dalPerfil = (DAL_Perfil)_repositorio;


            dalPerfil.EliminarRelacionesPorPerfil(idPerfil);

            if (nuevosIdsPermisos != null && nuevosIdsPermisos.Count > 0)
            {
                foreach (int idPermiso in nuevosIdsPermisos)
                {
                    dalPerfil.GuardarRelacion(idPerfil, idPermiso);
                }
            }
            RegistrarEventoBitacora($"Se sincronizaron los permisos del Perfil ID: {idPerfil}", 2);
        }

      

        /// <summary>
        /// Obtiene todos los permisos finales (desplegando las familias) 
        /// para saber qué puede hacer realmente un usuario con este perfil.
        /// </summary>
        public List<ComponentePermiso> ObtenerPermisosDePerfil(int idPerfil)
        {
            if (idPerfil <= 0)
                return new List<ComponentePermiso>();

            DAL_Perfil dalPerfil = (DAL_Perfil)_repositorio;


            return dalPerfil.ObtenerPermisosAtomicosDePerfil(idPerfil);
        }


        #region Auditoría de CRUD (Overrides)

        public override void Insertar(Perfil entidad)
        {
            base.Insertar(entidad);
            RegistrarEventoBitacora($"Se creó un nuevo perfil: {entidad.Nombre}", 1);
        }

        public override void Modificar(Perfil entidad)
        {
            base.Modificar(entidad);
            RegistrarEventoBitacora($"Se modificó el perfil: {entidad.Nombre}", 2);
        }

        public override void Eliminar(int id)
        {
            var perfilAEliminar = base.ObtenerPorId(id);
            string detalle = perfilAEliminar != null ? perfilAEliminar.Nombre : $"ID {id}";

            base.Eliminar(id);
            RegistrarEventoBitacora($"Se eliminó el perfil: {detalle}", 3);
        }

        #endregion

        #region Métodos Privados

        private void RegistrarEventoBitacora(string mensaje, int criticidad = 2)
        {
            try
            {
                string usuario = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
                _bllEvento.Registrar(usuario, ModulosEventos.Seguridad, mensaje, criticidad);
            }
            catch { }
        }

        #endregion
    }
}
