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

        private DAL_Mejora Repo => (DAL_Mejora)_repositorio;

        public void CargarStock(Mejora mejora, int cantidad)
        {
            mejora.Stock += cantidad;
            this.Modificar(mejora);
        }

        public bool ValidarStockDisponible(Mejora m)
        {
            return m.Stock > 0;
        }

        public Mejora ObtenerPorClaveHtml(string claveHtml)
        {
            if (string.IsNullOrEmpty(claveHtml)) return null;

            string nombreBuscado = string.Empty;


            switch (claveHtml.Trim().ToLower())
            {
                case "aleron":
                    nombreBuscado = "Alerón Deportivo";
                    break;
                case "kitcarroceria":
                    nombreBuscado = "Kit de Carrocería";
                    break;
                case "llantas":
                    nombreBuscado = "Llantas de Aleación";
                    break;
                case "suspension":
                    nombreBuscado = "Suspensión Deportiva";
                    break;
                case "pintura":
                    nombreBuscado = "Pintura Especial";
                    break;
                default:
                    return null;
            }


            List<Mejora> todas = this.Listar();
            return todas.Find(m => m.Nombre.Equals(nombreBuscado, StringComparison.OrdinalIgnoreCase));
        }

        public Mejora ObtenerPorClaveExtra(string claveExtra)
        {
            if (string.IsNullOrEmpty(claveExtra)) return null;

            string nombreBusqueda = claveExtra.Trim().ToLower();


            switch (nombreBusqueda)
            {
                case "aleron": nombreBusqueda = "Alerón"; break;
                case "kitcarroceria": nombreBusqueda = "Kit de Carrocería"; break;
                case "llantas": nombreBusqueda = "Llantas"; break;
                case "suspension": nombreBusqueda = "Suspensión"; break;
                case "pintura": nombreBusqueda = "Pintura"; break;
            }

            return Repo.ObtenerPorClave(nombreBusqueda);
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