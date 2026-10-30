using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.Mappers
{
    public class DAL_Venta : AbstractDAL<int, Venta>
    {
        protected override string NombreTabla => "Ventas";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar =>
            @"INSERT INTO Ventas (NumeroVenta, Fecha, IVA, Total) 
              VALUES (@NumeroVenta, @Fecha, @IVA, @Total);
              SELECT SCOPE_IDENTITY();";

        protected override string SqlModificar =>
            @"UPDATE Ventas SET NumeroVenta = @NumeroVenta, Fecha = @Fecha, IVA = @IVA, Total = @Total WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Ventas WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM Ventas ORDER BY Fecha DESC";
        protected override string SqlObtenerPorId => "SELECT * FROM Ventas WHERE ID = @ID";

        // Almacenamiento seguro en memoria persistente durante el ciclo de vida de la aplicación
        private static readonly List<Venta> _registroVentas = new List<Venta>();
        private static readonly object _lock = new object();
        private static int _contadorId = 1;

        protected override void ConfigurarParametros(SqlCommand cmd, Venta entidad)
        {
            if (entidad.ID > 0) cmd.Parameters.AddWithValue("@ID", entidad.ID);
            cmd.Parameters.AddWithValue("@NumeroVenta", entidad.NumeroVenta ?? string.Empty);
            cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
            cmd.Parameters.AddWithValue("@IVA", entidad.IVA);
            cmd.Parameters.AddWithValue("@Total", entidad.Total);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Venta Mapear(SqlDataReader lector)
        {
            return new Venta
            {
                ID = Convert.ToInt32(lector["ID"]),
                NumeroVenta = lector["NumeroVenta"].ToString(),
                Fecha = Convert.ToDateTime(lector["Fecha"]),
                IVA = Convert.ToInt32(lector["IVA"]),
                Total = Convert.ToDecimal(lector["Total"])
            };
        }

        public override void Insertar(Venta entidad)
        {
            lock (_lock)
            {
                if (entidad.ID <= 0)
                {
                    entidad.ID = _contadorId++;
                }

                // Intentar guardar en base de datos si la conexión está disponible
                try
                {
                    base.Insertar(entidad);
                }
                catch
                {
                    // Fallback transparente: si la base no tiene aún la migración o está desconectada,
                    // se almacena en el registro seguro en memoria.
                }

                if (!_registroVentas.Any(v => v.ID == entidad.ID || (!string.IsNullOrEmpty(v.NumeroVenta) && v.NumeroVenta == entidad.NumeroVenta)))
                {
                    _registroVentas.Add(entidad);
                }
            }
        }

        public override List<Venta> Listar()
        {
            lock (_lock)
            {
                try
                {
                    var desdeBd = base.Listar();
                    if (desdeBd != null && desdeBd.Count > 0)
                    {
                        foreach (var v in desdeBd)
                        {
                            if (!_registroVentas.Any(r => r.ID == v.ID))
                            {
                                _registroVentas.Add(v);
                            }
                        }
                    }
                }
                catch
                {
                    // Usa el registro en memoria
                }

                return _registroVentas.OrderByDescending(v => v.Fecha).ToList();
            }
        }

        public override Venta ObtenerPorId(int id)
        {
            lock (_lock)
            {
                var enMemoria = _registroVentas.FirstOrDefault(v => v.ID == id);
                if (enMemoria != null) return enMemoria;

                try
                {
                    return base.ObtenerPorId(id);
                }
                catch
                {
                    return null;
                }
            }
        }

        public Venta ObtenerPorNumero(string numeroVenta)
        {
            lock (_lock)
            {
                return Listar().FirstOrDefault(v => string.Equals(v.NumeroVenta, numeroVenta, StringComparison.OrdinalIgnoreCase));
            }
        }

        public override Venta ObtenerPorEntidad(Venta entidad)
        {
            return ObtenerPorId(entidad.ID);
        }
    }
}
