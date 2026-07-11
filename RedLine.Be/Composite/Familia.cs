using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios.Composite
{
    public class Familia : ComponentePermiso
    {
        public Familia(int id, string nombre) : base(id, nombre)
        {
        }

        protected Familia() { }

        public override void Agregar(ComponentePermiso componente)
        {
            if (!ComponentesHijos.Any(c => c.Id == componente.Id))
            {
                ComponentesHijos.Add(componente);
            }
        }

        public override void Quitar(ComponentePermiso componente)
        {
            var item = ComponentesHijos.FirstOrDefault(c => c.Id == componente.Id);
            if (item != null)
                ComponentesHijos.Remove(item);
        }

        public override IEnumerable<ComponentePermiso> ObtenerHijos()
        {
            var todosLosPermisos = new List<ComponentePermiso>();
            foreach (var hijo in ComponentesHijos)
            {
                todosLosPermisos.AddRange(hijo.ObtenerHijos());
            }
            return todosLosPermisos;
        }
    }
}
