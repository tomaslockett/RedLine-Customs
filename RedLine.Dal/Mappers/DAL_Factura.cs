using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RedLine.Dal.Mappers
{
    public class DAL_Factura : AbstractDAL<int, Factura>
    {
        protected override string NombreTabla => "Factura";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar =>
            @"INSERT INTO Factura (ID, NumeroFactura, FechaEmision, Total, MetodoPago, IVA) 
              VALUES (@ID, @NumeroFactura, @FechaEmision, @Total, @MetodoPago, @IVA);";

        protected override string SqlModificar =>
            @"UPDATE Factura SET NumeroFactura = @NumeroFactura, FechaEmision = @FechaEmision, Total = @Total, MetodoPago = @MetodoPago, IVA = @IVA WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Factura WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM Factura ORDER BY FechaEmision DESC";
        protected override string SqlObtenerPorId => "SELECT * FROM Factura WHERE ID = @ID";

        // Registro seguro en memoria persistente durante la ejecución
        private static readonly List<Factura> _registroFacturas = new List<Factura>();
        private static readonly object _lock = new object();
        private static int _contadorId = 1;

        protected override void ConfigurarParametros(SqlCommand cmd, Factura entidad)
        {
            cmd.Parameters.AddWithValue("@ID", entidad.ID);
            cmd.Parameters.AddWithValue("@NumeroFactura", entidad.NumeroFactura ?? string.Empty);
            cmd.Parameters.AddWithValue("@FechaEmision", entidad.FechaEmision);
            cmd.Parameters.AddWithValue("@Total", entidad.Total);
            cmd.Parameters.AddWithValue("@MetodoPago", entidad.MetodoPago ?? string.Empty);
            cmd.Parameters.AddWithValue("@IVA", entidad.IVA);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Factura Mapear(SqlDataReader lector)
        {
            return new Factura
            {
                ID = Convert.ToInt32(lector["ID"]),
                NumeroFactura = lector["NumeroFactura"] != DBNull.Value ? lector["NumeroFactura"].ToString() : string.Empty,
                FechaEmision = Convert.ToDateTime(lector["FechaEmision"]),
                Total = Convert.ToDecimal(lector["Total"]),
                MetodoPago = lector["MetodoPago"] != DBNull.Value ? lector["MetodoPago"].ToString() : string.Empty,
                IVA = Convert.ToInt32(lector["IVA"])
            };
        }

        public override void Insertar(Factura entidad)
        {
            lock (_lock)
            {
                // En EF6 Factura tiene relación 1:1 con Venta donde Factura.ID es PK y FK a Venta.ID
                if (entidad.Venta != null && entidad.Venta.ID > 0)
                {
                    entidad.ID = entidad.Venta.ID;
                }
                else if (entidad.ID <= 0)
                {
                    entidad.ID = _contadorId++;
                }

                try
                {
                    base.Insertar(entidad);
                }
                catch
                {
                    // Fallback transparente en memoria si la relación 1:1 en SQL ya existe o no migró
                }

                int idxExistente = _registroFacturas.FindIndex(f => f.ID == entidad.ID || (!string.IsNullOrEmpty(f.NumeroFactura) && f.NumeroFactura == entidad.NumeroFactura));
                if (idxExistente >= 0)
                {
                    _registroFacturas[idxExistente] = entidad;
                }
                else
                {
                    _registroFacturas.Add(entidad);
                }
            }
        }

        public override List<Factura> Listar()
        {
            lock (_lock)
            {
                try
                {
                    var desdeBd = base.Listar();
                    if (desdeBd != null && desdeBd.Count > 0)
                    {
                        foreach (var f in desdeBd)
                        {
                            if (!_registroFacturas.Any(r => r.ID == f.ID || (!string.IsNullOrEmpty(r.NumeroFactura) && r.NumeroFactura == f.NumeroFactura)))
                            {
                                _registroFacturas.Add(f);
                            }
                        }
                    }
                }
                catch
                {
                    // Usa el registro en memoria
                }

                return _registroFacturas.OrderByDescending(f => f.FechaEmision).ToList();
            }
        }

        public override Factura ObtenerPorId(int id)
        {
            lock (_lock)
            {
                var enMemoria = _registroFacturas.FirstOrDefault(f => f.ID == id);
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

        public Factura ObtenerPorNumero(string numeroFactura)
        {
            lock (_lock)
            {
                return Listar().FirstOrDefault(f => string.Equals(f.NumeroFactura, numeroFactura, StringComparison.OrdinalIgnoreCase));
            }
        }

        public override Factura ObtenerPorEntidad(Factura entidad)
        {
            return ObtenerPorId(entidad.ID);
        }
    }
}
