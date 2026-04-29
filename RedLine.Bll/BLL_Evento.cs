using RedLine.Dal.Mappers;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_Evento : AbstractBLL<int, Evento>
    {
        public BLL_Evento() : base(new DAL_Evento()) { }

        public void Registrar(string usuario, string modulo, string actividad, int criticidad = 1)
        {
            Evento nuevo = new Evento(usuario, modulo, actividad, criticidad);
            this.Insertar(nuevo);
        }

        public List<Evento> ListarTodo()
        {
            return this.Listar();
        }
    }
}
