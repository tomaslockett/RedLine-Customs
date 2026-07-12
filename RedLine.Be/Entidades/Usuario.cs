using System;
using RedLine.Servicios.Composite;

namespace Redline.Be
{
    public class Usuario
    {
        public int ID { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public virtual Perfil Perfil { get; set; }
        public int? PerfilId { get; set; }
        public int Intentos { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }
        public DateTime UltimoIntento { get; set; }
        public Usuario(int id, string dni, string nombre, string apellido, string email, string contraseña, Perfil perfil, int intentos, bool bloqueado, bool activo, DateTime ultimoIntento)
        {
            this.ID = id;
            this.Email = email;
            this.Contraseña = contraseña;
            this.Perfil = perfil;
            this.Intentos = intentos;
            this.Bloqueado = bloqueado;
            this.Activo = activo;
            this.UltimoIntento = ultimoIntento;
            this.DNI = dni;
            this.Nombre = nombre;
            this.Apellido = apellido;
        }
        public Usuario(int id, string dni, string nombre, string apellido, string email, string contraseña, int? perfilId, int intentos, bool bloqueado, bool activo, DateTime ultimoIntento)
        {
            this.ID = id;
            this.Email = email;
            this.Contraseña = contraseña;
            this.PerfilId = perfilId; 
            this.Intentos = intentos;
            this.Bloqueado = bloqueado;
            this.Activo = activo;
            this.UltimoIntento = ultimoIntento;
            this.DNI = dni;
            this.Nombre = nombre;
            this.Apellido = apellido;

            if (perfilId.HasValue)
            {
                this.Perfil = new Perfil { Id = perfilId.Value };
            }
            else
            {
                this.Perfil = null;
            }
        }
        public Usuario() { }
    }
}
