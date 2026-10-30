using Redline.Be;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace RedLine.Bll
{
    public class BLL_Cliente : AbstractBLL<string, Cliente>
    {
        public BLL_Cliente() : base(new DAL_Cliente()) { }

        private BLL_Evento _bllEvento = new BLL_Evento();
        private BLL_Usuario _bllUsuario = new BLL_Usuario();
        private BLL_Perfil _bllPerfil = new BLL_Perfil();

        #region Auditoría de CRUD (Overrides)

        public override void Insertar(Cliente cliente)
        {

            string dniPlano = cliente.DNI;
            string nombrePlano = cliente.Nombre;
            string apellidoPlano = cliente.Apellido;
            string emailPlano = cliente.Email;
            string passPlana = cliente.Contraseña;
            string passHasheada = AsegurarSha256(passPlana);


            cliente.Nombre = Hashing.EncriptarAES(nombrePlano);
            cliente.Apellido = Hashing.EncriptarAES(apellidoPlano);
            cliente.DNI = Hashing.EncriptarAES(dniPlano);
            cliente.Telefono = Hashing.EncriptarAES(cliente.Telefono ?? string.Empty);
            cliente.Direccion = Hashing.EncriptarAES(cliente.Direccion ?? string.Empty);
            cliente.Contraseña = passHasheada;


            base.Insertar(cliente);

 
            bool existeUsuario = _bllUsuario.Listar().Any(u => u.Email.Equals(emailPlano, StringComparison.OrdinalIgnoreCase));
            if (!existeUsuario)
            {

                var perfilCliente = _bllPerfil.Listar().FirstOrDefault(p => p.Nombre.Equals("Cliente", StringComparison.OrdinalIgnoreCase));
                int idPerfilCliente = perfilCliente != null ? perfilCliente.Id : 23;

                Usuario nuevoUsuario = new Usuario
                {
                    DNI = dniPlano,
                    Nombre = nombrePlano,
                    Apellido = apellidoPlano,
                    Email = emailPlano,
                    Contraseña = passPlana, 
                    PerfilId = idPerfilCliente,
                    Intentos = 0,
                    Bloqueado = false,
                    Activo = true,
                    UltimoIntento = DateTime.Now
                };

                _bllUsuario.Insertar(nuevoUsuario);
            }

            RegistrarEventoBitacora(emailPlano, $"Registro de nuevo cliente: {emailPlano}", 1);
        }

        public override void Modificar(Cliente cliente)
        {
            string dniPlano = cliente.DNI;
            string nombrePlano = cliente.Nombre;
            string apellidoPlano = cliente.Apellido;
            string emailPlano = cliente.Email;

            cliente.Nombre = Hashing.EncriptarAES(nombrePlano);
            cliente.Apellido = Hashing.EncriptarAES(apellidoPlano);
            cliente.DNI = Hashing.EncriptarAES(dniPlano);
            cliente.Telefono = Hashing.EncriptarAES(cliente.Telefono ?? string.Empty);
            cliente.Direccion = Hashing.EncriptarAES(cliente.Direccion ?? string.Empty);

            if (!string.IsNullOrEmpty(cliente.Contraseña))
            {
                cliente.Contraseña = AsegurarSha256(cliente.Contraseña);
            }

            base.Modificar(cliente);

            var usuarioAsociado = _bllUsuario.Listar().FirstOrDefault(u => u.Email.Equals(emailPlano, StringComparison.OrdinalIgnoreCase));
            if (usuarioAsociado != null)
            {
                usuarioAsociado.DNI = dniPlano;
                usuarioAsociado.Nombre = nombrePlano;
                usuarioAsociado.Apellido = apellidoPlano;
                if (!string.IsNullOrEmpty(cliente.Contraseña))
                {
                    usuarioAsociado.Contraseña = cliente.Contraseña;
                }
                _bllUsuario.Modificar(usuarioAsociado);
            }

            RegistrarEventoBitacora(emailPlano, $"Modificación de datos sensibles del cliente: {emailPlano}", 2);
        }

        public override void Eliminar(string emailODni)
        {
            var todosLosClientes = ObtenerClientes();
            var clienteAEliminar = todosLosClientes.FirstOrDefault(c =>
                c.Email.Equals(emailODni, StringComparison.OrdinalIgnoreCase) ||
                c.DNI.Equals(emailODni, StringComparison.OrdinalIgnoreCase));

            if (clienteAEliminar != null)
            {
                string dniEncriptado = Hashing.EncriptarAES(clienteAEliminar.DNI);
                base.Eliminar(dniEncriptado);

                var usuarioAsociado = _bllUsuario.Listar().FirstOrDefault(u =>
                    u.Email.Equals(clienteAEliminar.Email, StringComparison.OrdinalIgnoreCase));

                if (usuarioAsociado != null)
                {
                    _bllUsuario.Eliminar(usuarioAsociado.ID);
                }

                RegistrarEventoBitacora(clienteAEliminar.Email, $"Se eliminó la cuenta del cliente: {clienteAEliminar.Email}", 3);
            }
            else
            {
                base.Eliminar(emailODni);
                RegistrarEventoBitacora(emailODni, $"Se eliminó la cuenta del cliente: {emailODni}", 3);
            }
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
                //c.DNI = Hashing.DesencriptarAES(c.DNI);
                c.DNI = c.DNI;
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

        private string AsegurarSha256(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (input.Length == 64 && Regex.IsMatch(input, "^[0-9a-fA-F]{64}$"))
                return input.ToLower();

            return Hashing.Sha256(input);
        }

        private void RegistrarEventoBitacora(string ejecutante, string mensaje, int criticidad = 2)
        {
            try
            {
                string usuario = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : ejecutante;
                _bllEvento.Registrar(usuario, ModulosEventos.Clientes, mensaje, criticidad);
            }
            catch
            {
            }
        }

        #endregion
    }
}
