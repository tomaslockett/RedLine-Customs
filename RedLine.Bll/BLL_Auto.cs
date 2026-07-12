using Redline.Be;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Auto : AbstractBLL<int, AutoPersonalizado>
    {
        private DAL_AutoPersonalizado _dalAuto;
        private DAL_AutoBase _dalBase;
        private BLL_Evento _bllEvento;

        public BLL_Auto() : base(new DAL_AutoPersonalizado())
        {
            _dalAuto = (DAL_AutoPersonalizado)_repositorio;
            _dalBase = new DAL_AutoBase();
            _bllEvento = new BLL_Evento();
        }

        public void GuardarAutoEnGarage(AutoPersonalizado auto)
        {
            if (string.IsNullOrEmpty(auto.DNI_Cliente))
            {
                throw new Exception("El auto debe tener un cliente asignado.");
            }

            if (auto.Mejoras.Count == 0)
            {
                throw new Exception("Un auto personalizado debe tener al menos una mejora.");
            }

            _dalAuto.GuardarAutoCompleto(auto);

            RegistrarEventoBitacora($"Auto personalizado guardado en el garage para el cliente DNI: {auto.DNI_Cliente}", 1);
        }
        public void GuardarAuto(AutoBase auto)
        {
            _dalBase.Insertar(auto);

            RegistrarEventoBitacora($"Se dio de alta un nuevo Auto Base en el sistema: {auto.Marca} {auto.Modelo}", 1);
        }
        public AutoBase DevolverAuto(int id)
        {
            return _dalBase.ObtenerPorId(id);
        }

        //Lois
        //por ahora no use DTOs, pasa que casi q en el DTO pondria lo mismo q en la clase normal
        public List<AutoBase> MostrarAutosBase()
        {
            return _dalBase.Listar();
        }

        #region Auditoría de CRUD (Overrides para AutoPersonalizado)

        public override void Insertar(AutoPersonalizado entidad)
        {
            base.Insertar(entidad);
            RegistrarEventoBitacora($"Se insertó un registro de auto personalizado para el DNI: {entidad.DNI_Cliente}", 1);
        }

        public override void Modificar(AutoPersonalizado entidad)
        {
            base.Modificar(entidad);
            RegistrarEventoBitacora($"Se modificaron los datos del auto personalizado (Decorator) ID: {entidad.Id}", 2);
        }

        public override void Eliminar(int id)
        {
            base.Eliminar(id);
            RegistrarEventoBitacora($"Se eliminó el auto personalizado ID: {id} del taller", 3);
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
            catch
            {
            }
        }

        #endregion
    }
}