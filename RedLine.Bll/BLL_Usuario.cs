using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redline.Servicios;

namespace RedLine.Bll
{
    public class BLL_Usuario
    {
        private DAL_Usuario _usuarioDAL = new DAL_Usuario();

        public LoginResult Login(string username, string contraseña)
        {
            if (SessionManager.Instancia.IsLogged()) throw new LoginException(LoginResult.UserAlreadyLoggedIn);

            Usuario user = _usuarioDAL.ObtenerPorUsername(username);

            if (user == null) throw new LoginException(LoginResult.InvalidUsername);

            if (user.Bloqueado)
            {
                TimeSpan dif = DateTime.Now - user.UltimoIntento;
                if (dif.TotalHours < 4)
                {
                    double rest = 4 - dif.TotalHours;
                    string msg = rest < 1 ? $"Reintente en {(int)(rest * 60)} min" : $"Reintente en {rest:F1} hs";
                    throw new LoginException(LoginResult.UserBlocked, msg);
                }
                user.Bloqueado = false;
                user.Intentos = 0;
            }

            if (user.Activo == false) throw new Exception(LoginResult.UserInactive);

            string passHasheada = Hashing.HashearPassword(contraseña);
            if (!user.Contraseña.Equals(passHasheada))
            {
                user.Intentos++;
                user.UltimoIntento = DateTime.Now;
                if (user.Intentos >= 3) user.Bloqueado = true;

                _usuarioDAL.ActualizarUsuario(user);
                throw new LoginException(LoginResult.InvalidPassword);
            }

            user.Intentos = 0;
            user.UltimoIntento = DateTime.Now;
            _usuarioDAL.ActualizarUsuario(user);
            SessionManager.Instancia.Login(user);
            return LoginResult.ValidUser;
        }

        public void AltaUsuario(Usuario usuario)
        {
            usuario.Contraseña = Hashing.HashearPassword(usuario.Contraseña);
            _usuarioDAL.AltaUsuario(usuario);
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            _usuarioDAL.ActualizarUsuario(usuario);
        }

        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            return usuarios;
        }

        public void DesbloquearUsuario(Usuario usuario)
        {
            _usuarioDAL.DesbloquearUsuario(usuario);
            usuario.Bloqueado = false;
            usuario.Intentos = 0;
            _usuarioDAL.ActualizarUsuario(usuario);
        }

        public void Logout()
        {
            if (!SessionManager.Instancia.IsLogged()) throw new Exception("No hay sesión iniciada");
            SessionManager.Instancia.Logout();
        }

        public void Activar(Usuario usuario)
        {
            _usuarioDAL.Activar(usuario);
        }

        public void Desactivar(Usuario usuario)
        {
            _usuarioDAL.Desactivar(usuario);
        }
    }
}
