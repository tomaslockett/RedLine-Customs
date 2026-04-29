using System; 

namespace Redline.Be
{
    public class Usuario
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
        public int Intentos { get; set; }
        public bool Bloqueado { get; set; }
        public bool Activo { get; set; }
        public DateTime UltimoIntento { get; set; }
        public Usuario(int id, string email, string contraseña, string rol, int intentos, bool bloqueado, bool activo, DateTime ultimoIntento)
        {
            this.ID = id;
            this.Email = email;
            this.Contraseña = contraseña;
            this.Rol = rol;
            this.Intentos = intentos;
            this.Bloqueado = bloqueado;
            this.Activo = activo;
            this.UltimoIntento = ultimoIntento;
        }

        public Usuario() { }
    }
}
