using System;

namespace RedLine.Servicios
{
    public class SessionManager
    {
        private static SessionManager _instancia;
        private static readonly object _lock = new object();
        public Usuario Usuario { get; private set; }

        private SessionManager() { }

        public static SessionManager Instancia
        {
            get
            {
                lock (_lock)
                {
                    if (_instancia == null)
                    {
                        _instancia = new SessionManager();
                    }
                }
                return _instancia;
            }
        }

        public void Login(Usuario usuario)
        {
            Usuario = usuario;
        }

        public void Logout()
        {
            Usuario = null;
        }

        public bool IsLogged()
        {
            return Usuario != null;
        }
    }
}