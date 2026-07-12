using RedLine.Dal.Mappers;
using RedLine.Servicios;
using RedLine.Servicios.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_Familia : AbstractBLL<int, Familia>
    {
        private DAL_Familia _dalFamilia;
        private BLL_Evento _bllEvento;


        public BLL_Familia() : base(new DAL_Familia())
        {
            _dalFamilia = (DAL_Familia)_repositorio;
            _bllEvento = new BLL_Evento();
        }

        /// <summary>
        /// Sincroniza el árbol de permisos de una familia. 
        /// Borra las relaciones anteriores y guarda el nuevo listado de hijos.
        /// </summary>
        public void GuardarRelacionesFamilia(Familia familia)
        {
            if (familia == null || familia.Id <= 0)
                throw new Exception("La familia no es válida para asignar relaciones.");

            _dalFamilia.EliminarRelacionesPorFamilia(familia.Id);

            if (familia.ComponentesHijos != null && familia.ComponentesHijos.Count > 0)
            {
                foreach (var hijo in familia.ComponentesHijos)
                {
                    _dalFamilia.GuardarRelacionPadreHijo(familia.Id, hijo.Id);
                }
            }

            RegistrarEventoBitacora($"Se actualizaron los permisos/jerarquía de la Familia ID: {familia.Id}", 2);
        }

        #region Auditoría de CRUD (Overrides)

        public override void Insertar(Familia entidad)
        {
            base.Insertar(entidad);
            RegistrarEventoBitacora($"Se creó una nueva familia de permisos: {entidad.Nombre}", 1);
        }

        public override void Modificar(Familia entidad)
        {
            base.Modificar(entidad);
            RegistrarEventoBitacora($"Se modificó el nombre o datos de la familia: {entidad.Nombre}", 2);
        }

        public override void Eliminar(int id)
        {
            var familiaAEliminar = base.ObtenerPorId(id);
            string detalle = familiaAEliminar != null ? familiaAEliminar.Nombre : $"ID {id}";

            _dalFamilia.EliminarRelacionesPorFamilia(id);
            base.Eliminar(id);

            RegistrarEventoBitacora($"Se eliminó la familia de permisos: {detalle}", 3);
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
            catch
            {
            }
        }

        #endregion
    }
}

