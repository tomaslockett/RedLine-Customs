using Redline.Be;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Auto : AbstractBLL<int, AutoPersonalizado>
    {
        private DAL_AutoPersonalizado _dalAuto = new DAL_AutoPersonalizado();
        private DAL_AutoBase _dalBase = new DAL_AutoBase();
        private BLL_Evento _bllEvento = new BLL_Evento();

        public BLL_Auto() : base(new DAL_AutoPersonalizado()) { }

        public void GuardarAutoEnGarage(AutoPersonalizado auto)
        {
            if (string.IsNullOrEmpty(auto.DNI_Cliente))
                throw new Exception("El auto debe tener un cliente asignado.");

            if (auto.Mejoras.Count == 0)
                throw new Exception("Un auto personalizado debe tener al menos una mejora.");

            _dalAuto.GuardarAutoCompleto(auto);
            string u = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
            _bllEvento.Registrar(u, "Taller", $"Auto personalizado guardado para el cliente DNI: {auto.DNI_Cliente}", 1);
        }
        public void GuardarAuto(AutoBase auto) 
        {
            _dalBase.Insertar(auto);
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
    }
}