using System;
using System.Collections.Generic;
using Redline.Be;
using RedLine.Dal;

namespace RedLine.Bll
{
    public class BLL_Mejora : AbstractBLL<int, Mejora>
    {
        public BLL_Mejora() : base(new DAL_Mejora()) { }

        public void CargarStock(Mejora mejora, int cantidad)
        {
            mejora.Stock += cantidad;
            this.Modificar(mejora);
        }

        public bool ValidarStockDisponible(Mejora m)
        {
            return m.Stock > 0;
        }
    }
}