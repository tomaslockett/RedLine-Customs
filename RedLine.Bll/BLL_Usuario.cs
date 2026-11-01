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
        BLL_DigitoVerificador blldv = new BLL_DigitoVerificador();

        private DAL_Usuario Repo => (DAL_Usuario)_repositorio;

        public LoginResult Login(string email, string contraseña)
        {
            if (SessionManager.Instancia.IsLogged()) throw new LoginException(LoginResult.UserAlreadyLoggedIn);

            string estadoIntegridad = blldv.VerificarTodaLaBaseDeDatos();

            if (estadoIntegridad != "OK. La integridad de la base de datos es 100% correcta.")
            {
                return LoginInconsistente(email, contraseña);
            }

            Usuario user = Repo.ObtenerPorEmail(email);

            if (user == null)
            {
                _bllEvento.Registrar(email, ModulosEventos.Seguridad, "Intento de login con email inexistente", 2);
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

            string passHasheada = EsHashSha256(contraseña) ? contraseña.ToLower() : Hashing.Sha256(contraseña);
            if (!user.Contraseña.Equals(passHasheada, StringComparison.OrdinalIgnoreCase))
            {
                user.Intentos++;
                user.UltimoIntento = DateTime.Now;

                if (user.Intentos >= 3)
                {
                    user.Bloqueado = true;
                    _bllEvento.Registrar(email, ModulosEventos.Seguridad, "Usuario bloqueado por exceder intentos", 3);
                }
                else
                {
                    _bllEvento.Registrar(email, ModulosEventos.Seguridad, $"Intento de login fallido ({user.Intentos}/3)", 2);
                }

                this.Modificar(user);
                throw new LoginException(LoginResult.InvalidPassword);
            }

            user.Intentos = 0;
            user.UltimoIntento = DateTime.Now;
            this.Modificar(user);

            if (user.PerfilId.HasValue)
            {
                BLL_Perfil bllPerfil = new BLL_Perfil();
                user.Perfil = bllPerfil.ObtenerPorId(user.PerfilId.Value);
                var permisosDelUsuario = bllPerfil.ObtenerPermisosDePerfil(user.PerfilId.Value);
                foreach (var permiso in permisosDelUsuario)
                {
                    user.Perfil.AsignarPermiso(permiso);
                }
            }

            SessionManager.Instancia.Login(user);
            _bllEvento.Registrar(user.Email, ModulosEventos.Seguridad, "Inicio de sesión exitoso", 1);
            return LoginResult.ValidUser;
        }

        private LoginResult LoginInconsistente(string email, string contraseña)
        {
            Usuario user = Repo.ObtenerPorEmail(email);
            if (user == null) throw new LoginException(LoginResult.InvalidUsername);
            if (user.Bloqueado || !user.Activo) throw new LoginException(LoginResult.UserBlocked, "Tu usuario esta inactivo o bloqueado.");

            string passHasheada = EsHashSha256(contraseña) ? contraseña.ToLower() : Hashing.Sha256(contraseña);
            if (!user.Contraseña.Equals(passHasheada, StringComparison.OrdinalIgnoreCase)) throw new LoginException(LoginResult.InvalidPassword);

            if (user.PerfilId.HasValue)
            {
                BLL_Perfil bllPerfil = new BLL_Perfil();
                user.Perfil = bllPerfil.ObtenerPorId(user.PerfilId.Value);
            }

            if ((user.Perfil != null && user.Perfil.Nombre == "WebMaster") || user.Nombre == "Administrador")
            {
                SessionManager.Instancia.Login(user);
                return LoginResult.InconsistencyDVWebMaster;
            }

            return LoginResult.InconsistencyDVUserNormal;
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
                string claveInicial = usuario.Nombre.Substring(0, 3) + usuario.Apellido.Substring(0, 3) + usuario.DNI.Substring(0, 3);

                usuario.Contraseña = Hashing.Sha256(claveInicial);
            }
            else
            {
                // Si la contraseña ya viene hasheada en SHA-256 (64 caracteres hexadecimales),
                // no se vuelve a hashear para evitar el doble hasheo.
                if (EsHashSha256(usuario.Contraseña))
                {
                    usuario.Contraseña = usuario.Contraseña.ToLower();
                }
                else
                {
                    usuario.Contraseña = Hashing.Sha256(usuario.Contraseña);
                }
            }

            base.Insertar(usuario);

            string ejecutor = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
            _bllEvento.Registrar(ejecutor, ModulosEventos.Usuarios, $"Se creó un nuevo usuario con Email: {usuario.Email}", 1);
        }

        /// <summary>
        /// Verifica de forma simple si un texto ya es un hash SHA-256 válido (64 caracteres hexadecimales)
        /// para evitar hashear dos veces la misma contraseña.
        /// </summary>
        private bool EsHashSha256(string texto)
        {
            if (string.IsNullOrEmpty(texto) || texto.Length != 64)
            {
                return false;
            }

            foreach (char c in texto)
            {
                bool esNumero = (c >= '0' && c <= '9');
                bool esHexMinuscula = (c >= 'a' && c <= 'f');
                bool esHexMayuscula = (c >= 'A' && c <= 'F');

                if (!esNumero && !esHexMinuscula && !esHexMayuscula)
                {
                    return false;
                }
            }

            return true;
        }

        public override List<Usuario> Listar()
        {
            List<Usuario> usuarios = base.Listar();

            BLL_Perfil bllPerfil = new BLL_Perfil();

            foreach (Usuario u in usuarios)
            {
                if (u.PerfilId.HasValue)
                {
                    u.Perfil = bllPerfil.ObtenerPorId(u.PerfilId.Value);
                }
            }

            return usuarios;
        }

        public override void Modificar(Usuario usuario)
        {
            base.Modificar(usuario);
            string ejecutor = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
            _bllEvento.Registrar(ejecutor, ModulosEventos.Usuarios, $"Se actualizaron los datos del usuario: {usuario.Email}", 2);
        }

        public override void Eliminar(int id)
        {
            var userAEliminar = this.ObtenerPorId(id);
            base.Eliminar(id);
            string ejecutor = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : "Sistema";
            string detalle = userAEliminar != null ? userAEliminar.Email : $"ID {id}";
            _bllEvento.Registrar(ejecutor, ModulosEventos.Usuarios, $"Se eliminó el usuario: {detalle}", 3);
        }

        public override Usuario ObtenerPorId(int id)
        {
            Usuario u = base.ObtenerPorId(id);

            if (u != null && u.PerfilId.HasValue)
            {
                BLL_Perfil bllPerfil = new BLL_Perfil();
                u.Perfil = bllPerfil.ObtenerPorId(u.PerfilId.Value);
            }

            return u;
        }

        public void DesbloquearUsuario(Usuario usuario)
        {
            usuario.Bloqueado = false;
            usuario.Intentos = 0;
            string claveInicial = usuario.Nombre.Substring(0, 3) + usuario.Apellido.Substring(0, 3) + usuario.DNI.Substring(0, 3);
            usuario.Contraseña = Hashing.Sha256(claveInicial);
            this.Modificar(usuario);
            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, ModulosEventos.Usuarios, $"Desbloqueo de usuario y reset de clave: {usuario.Email}", 2);
        }

        public void Logout()
        {
            if (!SessionManager.Instancia.IsLogged()) throw new Exception("No hay sesión iniciada");
            string email = SessionManager.Instancia.Usuario.Email;
            SessionManager.Instancia.Logout();
            _bllEvento.Registrar(email, ModulosEventos.Seguridad, "Cierre de sesión", 1);
        }

        public void LogoutInconsistente()
        {
            if (!SessionManager.Instancia.IsLogged()) throw new Exception("No hay sesión iniciada");
            SessionManager.Instancia.Logout();
        }

        public void Activar(Usuario usuario)
        {
            usuario.Activo = true;
            this.Modificar(usuario);
            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, ModulosEventos.Usuarios, $"Activación de usuario: {usuario.Email}", 2);
        }

        public void Desactivar(Usuario usuario)
        {
            usuario.Activo = false;
            this.Modificar(usuario);
            string admin = SessionManager.Instancia.Usuario.Email;
            _bllEvento.Registrar(admin, ModulosEventos.Usuarios, $"Desactivación de usuario: {usuario.Email}", 2);
        }

        public void CambiarContraseñaDirecto(int idUsuario, string nuevaPasswordHasheada)
        {
            Repo.ActualizarContraseña(idUsuario, nuevaPasswordHasheada);
        }

        public void ActualizarIdiomaUsuario(int idUsuario, string nuevoIdioma)
        {
            if (string.IsNullOrEmpty(nuevoIdioma)) return;

            var usuario = ObtenerPorId(idUsuario);
            if (usuario != null)
            {
                usuario.Idioma = nuevoIdioma;
                this.Modificar(usuario);
            }
        }
    }
}
