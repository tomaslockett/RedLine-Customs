using RedLine.Be.Entidades;
using RedLine.Dal.Mappers;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Traduccion : AbstractBLL<int, Traduccion>
    {
        public BLL_Traduccion() : base(new DAL_Traduccion()) { }

        private DAL_Traduccion Repo => (DAL_Traduccion)_repositorio;

        public List<Traduccion> ListarPorIdiomaYPagina(int idIdioma, string pagina = null)
        {
            if (idIdioma <= 0)
            {
                throw new Exception("Debe seleccionar un idioma válido.");
            }

            return Repo.ListarPorIdiomaYPagina(idIdioma, string.IsNullOrWhiteSpace(pagina) ? null : pagina);
        }

        public void GuardarTraducciones(int idIdioma, List<Traduccion> listaTraducciones)
        {
            if (idIdioma <= 0)
            {
                throw new Exception("El idioma seleccionado no es válido.");
            }

            if (listaTraducciones == null || listaTraducciones.Count == 0)
            {
                return;
            }

            foreach (var traduccion in listaTraducciones)
            {
                Repo.GuardarOActualizar(idIdioma, traduccion.IdEtiqueta, traduccion.Texto);
            }
        }
    }
}
