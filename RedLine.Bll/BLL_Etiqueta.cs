using RedLine.Be.Entidades;
using RedLine.Dal.Mappers;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Etiqueta : AbstractBLL<int, Etiqueta>
    {
        public BLL_Etiqueta() : base(new DAL_Etiqueta()) { }

        private DAL_Etiqueta Repo => (DAL_Etiqueta)_repositorio;

        public override void Insertar(Etiqueta entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Clave))
            {
                throw new Exception("La clave de la etiqueta es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(entidad.Pagina))
            {
                throw new Exception("Debe especificar la página o módulo al que pertenece la etiqueta.");
            }


            base.Insertar(entidad);
        }

        public List<Etiqueta> ListarPorPagina(string pagina)
        {
            return Repo.ListarPorPagina(pagina);
        }
    }
}
