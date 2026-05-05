using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios.Composite
{
    public class Familia : ComponentePermiso
    {
        private readonly List<ComponentePermiso> Hijos = new List<ComponentePermiso>();

        public Familia(int id, string nombre) : base(id, nombre) 
        {
            
        }

        public override void Agregar(ComponentePermiso componente)
        {
            if (!Hijos.Any(c => c.Id == componente.Id))
            {
                Hijos.Add(componente);
            }
        }

        public override void Quitar(ComponentePermiso componente)
        {
            var item = Hijos.FirstOrDefault(c => c.Id == componente.Id);
            if (item != null) Hijos.Remove(item);
        }

        public override IEnumerable<ComponentePermiso> ObtenerHijos()
        {
            var todosLosPermisos = new List<ComponentePermiso>();
            foreach (var hijo in Hijos)
            {
                todosLosPermisos.AddRange(hijo.ObtenerHijos());
            }
            return todosLosPermisos;
        }
    }
}
