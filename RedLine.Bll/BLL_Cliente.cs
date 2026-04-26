using System;
using System.Collections.Generic;
using Redline.Be;
using RedLine.Dal;

namespace RedLine.Bll
{
    public class BLL_Cliente : AbstractBLL<string, Cliente>
    {
        public BLL_Cliente() : base(new DAL_Cliente()) { }

        public void AltaCliente(Cliente cliente)
        {
            this.Insertar(cliente);
        }

        public void ActualizarCliente(Cliente cliente)
        {
            this.Modificar(cliente);
        }

        public void BajaCliente(string dni)
        {
            this.Eliminar(dni);
        }

        public List<Cliente> ObtenerClientes()
        {
            return this.Listar();
        }

        public Cliente BuscarPorDNI(string dni)
        {
            return this.ObtenerPorId(dni);
        }
    }
}