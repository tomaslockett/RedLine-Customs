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
            return Repo.ObtenerTraduccionesPorIdioma(idIdioma);
        }

        public Idioma ObtenerDefault()
        {
            return Repo.ObtenerDefault();
        }

        public void GuardarNuevoIdioma(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("El nombre del idioma no puede estar vacío.");

            Idioma nuevoIdioma = new Idioma(0, nombre.Trim(), false);
            Insertar(nuevoIdioma);
        }

        public void GuardarTraduccion(int idIdioma, string clave, string texto)
        {
            if (string.IsNullOrWhiteSpace(clave)) throw new Exception("La clave de traducción no puede estar vacía.");
            Repo.GuardarTraduccion(idIdioma, clave, texto);
        }

        public void GuardarTraducciones(int idIdioma, Dictionary<string, string> traducciones)
        {
            if (traducciones == null) return;

            foreach (var kvp in traducciones)
            {
                GuardarTraduccion(idIdioma, kvp.Key, kvp.Value);
            }
        }
    }
}
