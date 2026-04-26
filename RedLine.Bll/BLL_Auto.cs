using System;
using System.Collections.Generic;
using Redline.Be;
using RedLine.Dal;

namespace RedLine.Bll
{
    public class BLL_Auto : AbstractBLL<int, AutoPersonalizado>
    {
        private DAL_AutoPersonalizado _dalAuto = new DAL_AutoPersonalizado();
        private DAL_AutoBase _dalBase = new DAL_AutoBase();

        public BLL_Auto() : base(new DAL_AutoPersonalizado()) { }

        public void GuardarAutoEnGarage(AutoPersonalizado auto)
        {
            if (string.IsNullOrEmpty(auto.DNI_Cliente))
                throw new Exception("El auto debe tener un cliente asignado.");

            if (auto.Mejoras.Count == 0)
                throw new Exception("Un auto personalizado debe tener al menos una mejora.");

            _dalAuto.GuardarAutoCompleto(auto);
        }
    }
}