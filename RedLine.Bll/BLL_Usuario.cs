using System;
using System.Collections.Generic;
using Redline.Be;
using RedLine.BLL;
using RedLine.Dal;
using RedLine.Servicios;

namespace RedLine.Bll
{
    public class BLL_Usuario : AbstractBLL<int, Usuario>
    {
        public BLL_Usuario() : base(new DAL_Usuario()) { }
        private BLL_Evento _bllEvento = new BLL_Evento();

        private DAL_Usuario Repo => (DAL_Usuario)_repositorio;

        public LoginResult Login(string email, string contraseña)
        {
            if (SessionManager.Instancia.IsLogged()) throw new LoginException(LoginResult.UserAlreadyLoggedIn);

            Usuario user = Repo.ObtenerPorEmail(email);

            if (user == null)
            {
                _bllEvento.Registrar(email, "Seguridad", "Intento de login con email inexistente", 2);
                throw new LoginException(LoginResult.InvalidUsername);
            }
                

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

            string passHasheada = Hashing.Sha256(contraseña);
            if (!user.Contraseña.Equals(passHasheada))
            {
                user.Intentos++;
                user.UltimoIntento = DateTime.Now;
                if (user.Intentos >= 3)
                {
                    user.Bloqueado = true;
                    _bllEvento.Registrar(email, "Seguridad", "Usuario bloqueado por exceder intentos", 3);
                }
                else
                {
                    _bllEvento.Registrar(email, "Seguridad", $"Intento de login fallido ({user.Intentos}/3)", 2);
                }

                this.Modificar(user);
                throw new LoginException(LoginResult.InvalidPassword);
            }

            user.Intentos = 0;
            user.UltimoIntento = DateTime.Now;
            this.Modificar(user);
            SessionManager.Instancia.Login(user);
            _bllEvento.Registrar(user.Email, "Seguridad", "Inicio de sesión exitoso", 1);
            return LoginResult.ValidUser;
        }

        public override void Insertar(Usuario usuario)
        {
            if (Repo.ObtenerPorDNI(usuario.DNI) != null)
            {
                throw new Exception("Ya existe un usuario registrado con ese DNI");
            }

            if (Repo.ObtenerPorEmail(usuario.Email) != null)
            {
                throw new Exception("El email ya se encuentra en uso");
            }

            if (string.IsNullOrEmpty(usuario.Contraseña))
            {
                string claveInicial = usuario.Nombre.Substring(0, 3) +usuario.Apellido.Substring(0, 3) +usuario.DNI.Substring(0, 3);

                usuario.Contraseña = Hashing.Sha256(claveInicial);
            }
            else
            {
                usuario.Contraseña = Hashing.Sha256(usuario.Contraseña);
            }

            base.Insertar(usuario);

            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, "Usuarios", $"Desbloqueo manual y reset de clave para: {usuario.Email}", 2);
        }

        public void DesbloquearUsuario(Usuario usuario)
        {
            usuario.Bloqueado = false;
            usuario.Intentos = 0;
            string claveInicial = usuario.Nombre.Substring(0, 3) +
                                  usuario.Apellido.Substring(0, 3) +
                                  usuario.DNI.Substring(0, 3);

            usuario.Contraseña = Hashing.Sha256(claveInicial); 
            this.Modificar(usuario);
            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, "Usuarios", $"Desbloqueo de usuario: {usuario.Email}", 2);
        }

        public void Logout()
        {
            if (!SessionManager.Instancia.IsLogged()) throw new Exception("No hay sesión iniciada");
            string email = SessionManager.Instancia.Usuario.Email;
            SessionManager.Instancia.Logout();
            _bllEvento.Registrar(email, "Seguridad", "Cierre de sesión", 1);
        }

        public void Activar(Usuario usuario)
        {
            usuario.Activo = true;
            this.Modificar(usuario);
            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, "Usuarios", $"Activación de usuario: {usuario.Email}", 2);
        }

        public void Desactivar(Usuario usuario)
        {
            usuario.Activo = false;
            this.Modificar(usuario);
            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, "Usuarios", $"Desactivación de usuario: {usuario.Email}", 2);
        }

        public void CambiarContraseñaDirecto(int idUsuario, string nuevaPasswordHasheada)
        {
            Repo.ActualizarContraseña(idUsuario, nuevaPasswordHasheada);
        }
    }
}
