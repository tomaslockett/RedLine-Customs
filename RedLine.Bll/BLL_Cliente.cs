using Redline.Be;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RedLine.Bll
{
    public class BLL_Cliente : AbstractBLL<string, Cliente>
    {
        public BLL_Cliente() : base(new DAL_Cliente()) { }
        private BLL_Evento _bllEvento = new BLL_Evento();

        #region Auditoría de CRUD (Overrides)

        public override void Insertar(Cliente cliente)
        {
            cliente.Nombre = Hashing.EncriptarAES(cliente.Nombre);
            cliente.Apellido = Hashing.EncriptarAES(cliente.Apellido);
            cliente.DNI = Hashing.EncriptarAES(cliente.DNI);
            cliente.Telefono = Hashing.EncriptarAES(cliente.Telefono);
            cliente.Direccion = Hashing.EncriptarAES(cliente.Direccion);
            cliente.Contraseña = Hashing.Sha256(cliente.Contraseña);

            base.Insertar(cliente);

            RegistrarEventoBitacora($"Registro de nuevo cliente: {cliente.Email}", 1);
        }

        public override void Modificar(Cliente cliente)
        {
            cliente.Nombre = Hashing.EncriptarAES(cliente.Nombre);
            cliente.Apellido = Hashing.EncriptarAES(cliente.Apellido);
            cliente.DNI = Hashing.EncriptarAES(cliente.DNI);
            cliente.Telefono = Hashing.EncriptarAES(cliente.Telefono);
            cliente.Direccion = Hashing.EncriptarAES(cliente.Direccion);

            base.Modificar(cliente);

            RegistrarEventoBitacora($"Modificación de datos sensibles del cliente: {cliente.Email}", 2);
        }

        public override void Eliminar(string email)
        {
            var clienteAEliminar = BuscarPorEmail(email);
            string detalle = clienteAEliminar != null ? clienteAEliminar.Email : email;

            base.Eliminar(email);

            RegistrarEventoBitacora($"Se eliminó la cuenta del cliente: {detalle}", 3);
        }

        #endregion

        #region Métodos de Negocio

        public int ImportarClientesXML(Stream xmlStream)
        {
            if (xmlStream == null || xmlStream.Length == 0)
                throw new ArgumentException("El archivo XML está vacío.");

            XDocument doc;
            try
            {
                doc = XDocument.Load(xmlStream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("El archivo no tiene un formato XML válido.", ex);
            }

            List<Cliente> clientesExistentes = ObtenerClientes();


            HashSet<int> idsExistentes = new HashSet<int>(clientesExistentes.Select(c => c.ID));
            HashSet<string> emailsExistentes = new HashSet<string>(
                clientesExistentes.Select(c => c.Email),
                StringComparer.OrdinalIgnoreCase
            );

            List<Cliente> nuevosClientes = new List<Cliente>();
            var elementosCliente = doc.Descendants("Cliente");

            if (!elementosCliente.Any())
                throw new Exception("El XML no contiene nodos <Cliente>.");

            foreach (var elem in elementosCliente)
            {

                if (elem.Element("ID") == null || elem.Element("Email") == null)
                    continue;

                if (!int.TryParse(elem.Element("ID").Value, out int id))
                    continue;

                string email = elem.Element("Email").Value.Trim();


                if (idsExistentes.Contains(id) || emailsExistentes.Contains(email))
                    continue;

                Cliente nuevoCliente = new Cliente
                {
                    ID = id,
                    DNI = elem.Element("DNI")?.Value ?? string.Empty,
                    Nombre = elem.Element("Nombre")?.Value ?? string.Empty,
                    Apellido = elem.Element("Apellido")?.Value ?? string.Empty,
                    Email = email,
                    Contraseña = elem.Element("Contraseña")?.Value ?? string.Empty,
                    Telefono = elem.Element("Telefono")?.Value ?? string.Empty,
                    Direccion = elem.Element("Direccion")?.Value ?? string.Empty
                };

                nuevosClientes.Add(nuevoCliente);

                idsExistentes.Add(id);
                emailsExistentes.Add(email);
            }


            int guardados = 0;
            foreach (var cliente in nuevosClientes)
            {

                this.Insertar(cliente);
                guardados++;
            }

            return guardados;
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

        #endregion

        #region Métodos Privados

        private void RegistrarEventoBitacora(string mensaje, int criticidad = 2)
        {
            try
            {
                string usuario = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Auto-Registro";

                _bllEvento.Registrar(usuario, ModulosEventos.Clientes, mensaje, criticidad);
            }
            catch
            {
            }
        }

        #endregion
    }
}