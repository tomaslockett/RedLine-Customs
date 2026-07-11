using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios.Composite
{
    public class Permiso : ComponentePermiso
    {
        public Permiso(int id, string nombre) : base(id, nombre) 
        {
            
        }
        protected Permiso() { }
        public override void Agregar(ComponentePermiso componente)
        {
            throw new InvalidOperationException($"'{Nombre}' es un permiso atómico. No puede contener otros permisos.");
        }

        public override void Quitar(ComponentePermiso componente)
        {
            throw new InvalidOperationException($"'{Nombre}' no tiene componentes para quitar.");
        }

        public override IEnumerable<ComponentePermiso> ObtenerHijos()
        {
            yield return this;
        }
    }
}
