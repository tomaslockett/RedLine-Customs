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
            foreach (var m in auto.Mejoras)
            {
                if (m.Stock <= 0) throw new Exception($"Sin stock de: {m.Nombre}");
            }
            this.Insertar(auto);

            // Aquí llamarías a un método de la DAL de mejoras para restar el stock:
            // foreach(var m in auto.Mejoras) { _dalMejoras.RestarStock(m.ID); }
        }

        public List<AutoBase> ListarCatalogo()
        {
            return _dalBase.Listar();
        }

        public void EliminarDelGarage(int id)
        {
            this.Eliminar(id);
        }
    }
}