using Redline.Be;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Mejora : AbstractBLL<int, Mejora>
    {
        private BLL_Evento _bllEvento;
        public BLL_Mejora() : base(new DAL_Mejora()) 
        {
            _bllEvento = new BLL_Evento();
        }

        public void CargarStock(Mejora mejora, int cantidad)
        {
            mejora.Stock += cantidad;
            this.Modificar(mejora);
        }

        public bool ValidarStockDisponible(Mejora m)
        {
            return m.Stock > 0;
        }

        #region Auditoría de CRUD (Overrides)

        public override void Insertar(Mejora entidad)
        {
            base.Insertar(entidad);
            RegistrarEventoBitacora($"Se registró una nueva mejora/repuesto en el catálogo: {entidad.Nombre}", 1);
        }

        public override void Modificar(Mejora entidad)
        {
            base.Modificar(entidad);
            RegistrarEventoBitacora($"Se modificaron los datos o el stock de la mejora: {entidad.Nombre}", 2);
        }

        public override void Eliminar(int id)
        {
            var mejoraAEliminar = base.ObtenerPorId(id);
            string detalle = mejoraAEliminar != null ? mejoraAEliminar.Nombre : $"ID {id}";

            base.Eliminar(id);
            RegistrarEventoBitacora($"Se eliminó la mejora del catálogo: {detalle}", 3);
        }

        #endregion

        #region Métodos Privados

        private void RegistrarEventoBitacora(string mensaje, int criticidad = 2)
        {
            try
            {
                string usuario = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";

                _bllEvento.Registrar(usuario, ModulosEventos.Taller, mensaje, criticidad);
            }
            catch { }
        }

        #endregion
    }
}