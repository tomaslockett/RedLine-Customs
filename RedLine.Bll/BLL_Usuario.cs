using System;
using System.Collections.Generic;
using Redline.Be;
using RedLine.Dal;
using Redline.Servicios;

namespace RedLine.Bll
{
    public class BLL_Usuario : AbstractBLL<int, Usuario>
    {
        public BLL_Usuario() : base(new DAL_Usuario()) { }

        private DAL_Usuario Repo => (DAL_Usuario)_repositorio;

        public LoginResult Login(string username, string contraseña)
        {
            if (SessionManager.Instancia.IsLogged()) throw new LoginException(LoginResult.UserAlreadyLoggedIn);

            Usuario user = Repo.ObtenerPorUsername(username);

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

            if (!user.Activo) throw new Exception("Usuario inactivo");

            string passHasheada = Hashing.HashearPassword(contraseña);
            if (!user.Contraseña.Equals(passHasheada))
            {
                user.Intentos++;
                user.UltimoIntento = DateTime.Now;
                if (user.Intentos >= 3) user.Bloqueado = true;

                this.Modificar(user);
                throw new LoginException(LoginResult.InvalidPassword);
            }

            user.Intentos = 0;
            user.UltimoIntento = DateTime.Now;
            this.Modificar(user);
            SessionManager.Instancia.Login(user);
            return LoginResult.ValidUser;
        }

        public void AltaUsuario(Usuario usuario)
        {
            usuario.Contraseña = Hashing.HashearPassword(usuario.Contraseña);
            this.Insertar(usuario);
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            this.Modificar(usuario);
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return this.Listar();
        }

        public void DesbloquearUsuario(Usuario usuario)
        {
            usuario.Bloqueado = false;
            usuario.Intentos = 0;
            this.Modificar(usuario);
        }

        public void Logout()
        {
            if (!SessionManager.Instancia.IsLogged()) throw new Exception("No hay sesión iniciada");
            SessionManager.Instancia.Logout();
        }

        public void Activar(Usuario usuario)
        {
            usuario.Activo = true;
            this.Modificar(usuario);
        }

        public void Desactivar(Usuario usuario)
        {
            usuario.Activo = false;
            this.Modificar(usuario);
        }
    }
}
