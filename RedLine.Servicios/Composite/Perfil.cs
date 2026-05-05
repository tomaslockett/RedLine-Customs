using System.Collections.Generic;
using System.Linq;

namespace RedLine.Servicios.Composite
{
    public class Perfil
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        private readonly List<ComponentePermiso> PermisosRaiz = new List<ComponentePermiso>();

        public void AsignarPermiso(ComponentePermiso permiso)
        {
            PermisosRaiz.Add(permiso);
        }

        public List<ComponentePermiso> GetPermisosFinales()
        {
            return PermisosRaiz.SelectMany(p => p.ObtenerHijos()).GroupBy(p => p.Id) .Select(g => g.First()).ToList();
        }
    }
}
