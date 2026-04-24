using System; 

namespace Redline.Be
{
    public class Usuario
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
        public int Intentos { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime UltimoIntento { get; set; }
        public Usuario(int id, string username, string contraseña, string rol, int intentos, bool bloqueado, DateTime ultimoIntento)
        {
            this.ID = id;
            this.Username = username;
            this.Contraseña = contraseña;
            this.Rol = rol;
            this.Intentos = intentos;
            this.Bloqueado = bloqueado;
            this.UltimoIntento = ultimoIntento;
        }

        public Usuario() { }
    }
}
