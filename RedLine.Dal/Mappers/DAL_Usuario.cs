using Redline.Be;
using RedLine.Dal.ORM;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal
{
    // TODOS LOS MÉTODOS DE ESTE DAL DEBEN USAR LOS MÉTODOS BASE DE AbstractDAL (Insertar, Modificar, Eliminar, Listar, ObtenerPorId) PARA QUE SE RECALCULEN LOS DÍGITOS VERIFICADORES SI ES NECESARIO.

    // TODAS LAS DAL TIENE QUE ESTAR ASI COMO ESTA YA QUE PASAMOS POR LA ORM Y ASI SE RECALCULAN LOS DIGITOS VERIFICADORES, SI NO SE HACE ASI, NO SE RECALCULAN LOS DIGITOS VERIFICADORES Y SE ROMPE EL SISTEMA DE INTEGRIDAD DE DATOS.
    public class DAL_Usuario : AbstractDAL<int, Usuario>
    {
        // Nombre de la tabla en la base de datos. Es importante que coincida exactamente con el nombre real.
        protected override string NombreTabla => "";

        // Si no usaras dígitos verificadores, podrías poner false aquí y omitir la lógica relacionada en el AbstractDAL.
        protected override bool RequiereDigitoVerificador => true;

        // Queries para Insertar, Modificar, Eliminar, Listar y Obtener por ID.
        protected override string SqlInsertar => @"INSERT INTO Usuario (Username, Contraseña, Rol, Intentos, Bloqueado, UltimoIntento) VALUES (@Username, @Contraseña, @Rol, 0, 0, @UltimoIntento)";

        protected override string SqlModificar => @"UPDATE Usuario SET Username = @Username, Rol = @Rol, Bloqueado = @Bloqueado WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Usuario WHERE ID = @ID";

        protected override string SqlListar => "SELECT * FROM Usuario";

        protected override string SqlObtenerPorId => "SELECT * FROM Usuario WHERE ID = @ID";

        //Ensamblador de parametros: convierte un objeto Usuario en parámetros SQL para Insertar/Modificar
        protected override void ConfigurarParametros(SqlCommand cmd, Usuario e)
        {
            cmd.AgregarParametro("@ID", e.ID); 
            cmd.AgregarParametro("@Username", e.Username);
            cmd.AgregarParametro("@Contraseña", e.Contraseña);
            cmd.AgregarParametro("@Rol", e.Rol);
            cmd.AgregarParametroBooleano("@Bloqueado", e.Bloqueado);
            cmd.AgregarParametroFecha("@UltimoIntento", e.ID == 0 ? DateTime.Now : e.UltimoIntento);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.AgregarParametro("@ID", id);
        }

        // Lector: convierte un SqlDataReader en un objeto Usuario. Asegúrate de que los nombres de las columnas coincidan con los de tu base de datos.
        protected override Usuario Mapear(SqlDataReader lector)
        {
            return new Usuario(
                lector.Obtener<int>("ID"),
                lector.ObtenerCadenaSegura("Username"),
                lector.ObtenerCadenaSegura("Contraseña"),
                lector.ObtenerCadenaSegura("Rol"),
                lector.Obtener<int>("Intentos"),
                lector.Obtener<bool>("Bloqueado"),
                lector.Obtener<DateTime>("UltimoIntento")
            );
        }

        // Metodos adicionales específicos para Usuario. Por ejemplo, obtener por username o desbloquear usuario.
        public override Usuario ObtenerPorEntidad(Usuario entidad)
        {
            if (!string.IsNullOrEmpty(entidad.Username))
            {
                return ObtenerPorUsername(entidad.Username);
            }
            return null;
        }

        public Usuario ObtenerPorUsername(string username)
        {
            string query = "SELECT * FROM Usuario WHERE Username = @Username";

            using (var con = new SqlConnection(cx)) // cx viene de AbstractDAL
            {
                con.Open();
                var lista = con.EjecutarLectura(query, Mapear, cmd => cmd.AgregarParametro("@Username", username));
                return lista.FirstOrDefault();
            }
        }

        public void DesbloquearUsuario(Usuario usuario)
        {
            string query = "UPDATE Usuario SET Bloqueado = 0 WHERE ID = @id";

            using (var con = new SqlConnection(cx))
            {
                con.Open();
                con.EjecutarNoConsulta(query, cmd => cmd.AgregarParametro("@id", usuario.ID));
                // Nota del mecánico: Si quisieras que el desbloqueo recalcule el dígito verificador,
                // deberías llamar a base.Modificar(usuario) en lugar de hacer un UPDATE suelto.
            }
        }
    }
}
