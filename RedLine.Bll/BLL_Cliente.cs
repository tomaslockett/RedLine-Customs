using Redline.Be;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;

namespace RedLine.Bll
{
    public class BLL_Cliente : AbstractBLL<string, Cliente>
    {
        public BLL_Cliente() : base(new DAL_Cliente()) { }
        private BLL_Evento _bllEvento = new BLL_Evento();

        public override void Insertar(Cliente cliente)
        {
            cliente.Nombre = Hashing.EncriptarAES(cliente.Nombre);
            cliente.Apellido = Hashing.EncriptarAES(cliente.Apellido);
            cliente.DNI = Hashing.EncriptarAES(cliente.DNI);
            cliente.Telefono = Hashing.EncriptarAES(cliente.Telefono);
            cliente.Direccion = Hashing.EncriptarAES(cliente.Direccion);
            cliente.Contraseña = Hashing.Sha256(cliente.Contraseña);

            base.Insertar(cliente);
            string u = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Auto-Registro";
            _bllEvento.Registrar(u, "Clientes", $"Registro de nuevo cliente: {cliente.Email}", 1);
        }

        public override void Modificar(Cliente cliente)
        {
            cliente.Nombre = Hashing.EncriptarAES(cliente.Nombre);
            cliente.Apellido = Hashing.EncriptarAES(cliente.Apellido);
            cliente.DNI = Hashing.EncriptarAES(cliente.DNI);
            cliente.Telefono = Hashing.EncriptarAES(cliente.Telefono);
            cliente.Direccion = Hashing.EncriptarAES(cliente.Direccion);
            base.Modificar(cliente);
            string u = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : cliente.Email;
            _bllEvento.Registrar(u, "Clientes", $"Modificación de datos del cliente: {cliente.Email}", 2);
        }

        public List<Cliente> ObtenerClientes()
        {
            List<Cliente> lista = this.Listar();
            foreach (var c in lista)
            {
                c.Nombre = Hashing.DesencriptarAES(c.Nombre);
                c.Apellido = Hashing.DesencriptarAES(c.Apellido);
                c.DNI = Hashing.DesencriptarAES(c.DNI);
                c.Telefono = Hashing.DesencriptarAES(c.Telefono);
                c.Direccion = Hashing.DesencriptarAES(c.Direccion);
            }
            return lista;
        }

        public Cliente BuscarPorEmail(string email)
        {
            List<Cliente> clientes = ObtenerClientes();
            return clientes.Find(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public bool ExisteEmail(string email)
        {
            List<Cliente> clientes = this.Listar();
            return clientes.Exists(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public Cliente ObtenerDatosSesion()
        {
            if (SessionManager.Instancia.IsLogged())
            {
                string email = SessionManager.Instancia.Usuario.Email;
                return BuscarPorEmail(email);
            }
            return null;
        }
    }
}