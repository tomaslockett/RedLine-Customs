using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Servicios.Composite
{
    public abstract class ComponentePermiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<ComponentePermiso> ComponentesHijos { get; set; } = new List<ComponentePermiso>();
        protected ComponentePermiso(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
        protected ComponentePermiso() { }
        public abstract void Agregar(ComponentePermiso componente);
        public abstract void Quitar(ComponentePermiso componente);
        public abstract IEnumerable<ComponentePermiso> ObtenerHijos();
        public override string ToString() => Nombre;
    }
}
