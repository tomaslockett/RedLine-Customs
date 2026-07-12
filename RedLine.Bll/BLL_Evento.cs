using RedLine.Dal.Mappers;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_Evento : AbstractBLL<int, Evento>
    {
        public BLL_Evento() : base(new DAL_Evento()) { }

        public void Registrar(string usuario, ModulosEventos modulo, string actividad, int criticidad = 1)
        {
            string moduloFormateado = Regex.Replace(modulo.ToString(), "([a-z])([A-Z])", "$1 $2");

            Evento nuevo = new Evento(usuario, moduloFormateado, actividad, criticidad);
            this.Insertar(nuevo);
        }

        public List<Evento> ListarTodo()
        {
            return this.Listar();
        }
    }
}
