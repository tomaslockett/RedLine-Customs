using System;

namespace Redline.Be
{
    public class Cliente
    {
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public List<AutoPersonalizado> Garage { get; set; } = new List<AutoPersonalizado>();

        public Cliente() { }

        public Cliente(int id, string dni, string nombre, string apellido, string email, string telefono, string direccion)
        {
            this.ID = id;
            this.DNI = dni;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.Email = email;
            this.Telefono = telefono;
            this.Direccion = direccion;
        }
    }
}
