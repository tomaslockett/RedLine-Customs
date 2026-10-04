using RedLine.Be.Entidades;
using RedLine.Dal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_Idioma : AbstractBLL<int, Idioma>
    {
        public BLL_Idioma() : base(new DAL_Idioma()) { }

        private DAL_Idioma Repo => (DAL_Idioma)_repositorio;

        public Dictionary<string, string> ObtenerTraduccionesPorIdioma(int idIdioma)
        {
            if (idIdioma <= 0) return new Dictionary<string, string>();
            return Repo.ObtenerTraduccionesPorIdioma(idIdioma);
        }

        public Dictionary<string, string> ObtenerTraduccionesConFallback(int idIdioma)
        {
            return ObtenerTraduccionesPorIdioma(idIdioma);
        }

        public Idioma ObtenerDefault()
        {
            return Repo.ObtenerDefault();
        }

        public void GuardarNuevoIdioma(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del idioma no puede estar vacío.");

            string nombreLimpio = nombre.Trim();

            var idiomasExistentes = Listar();
            if (idiomasExistentes != null && idiomasExistentes.Any(i => i.Nombre.Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception($"Ya existe un idioma registrado con el nombre '{nombreLimpio}'.");
            }

            Idioma nuevoIdioma = new Idioma(0, nombreLimpio, false);
            Insertar(nuevoIdioma);
        }
    }
}
