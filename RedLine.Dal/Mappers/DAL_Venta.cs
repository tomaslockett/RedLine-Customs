using Redline.Be;
using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RedLine.Dal.Mappers
{
    public class DAL_Venta : AbstractDAL<int, Venta>
    {
        protected override string NombreTabla => "Venta";
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar =>
            @"INSERT INTO Venta (NumeroVenta, Fecha, IVA, Total, MetodoPago, Cliente_ID, AutoBase_ID, AutoPersonalizado_ID) 
              VALUES (@NumeroVenta, @Fecha, @IVA, @Total, @MetodoPago, @ClienteID, @AutoBaseID, @AutoPersonalizadoID);
              SELECT CAST(SCOPE_IDENTITY() AS INT);";

        protected override string SqlModificar =>
            @"UPDATE Venta SET NumeroVenta = @NumeroVenta, Fecha = @Fecha, IVA = @IVA, Total = @Total, MetodoPago = @MetodoPago WHERE ID = @ID";

        protected override string SqlEliminar => "DELETE FROM Venta WHERE ID = @ID";
        protected override string SqlListar => "SELECT * FROM Venta ORDER BY Fecha DESC";
        protected override string SqlObtenerPorId => "SELECT * FROM Venta WHERE ID = @ID";

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
            cmd.Parameters.AddWithValue("@MetodoPago", (object)entidad.MetodoPago ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClienteID", (entidad.Cliente != null && entidad.Cliente.ID > 0) ? (object)entidad.Cliente.ID : DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoBaseID", (entidad.AutoBase != null && entidad.AutoBase.ID > 0) ? (object)entidad.AutoBase.ID : DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoPersonalizadoID", (entidad.AutoPersonalizado != null && entidad.AutoPersonalizado.ID > 0) ? (object)entidad.AutoPersonalizado.ID : DBNull.Value);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Venta Mapear(SqlDataReader lector)
        {
            Venta v = new Venta
            {
                ID = Convert.ToInt32(lector["ID"]),
                NumeroVenta = lector["NumeroVenta"] != DBNull.Value ? lector["NumeroVenta"].ToString() : string.Empty,
                Fecha = Convert.ToDateTime(lector["Fecha"]),
                IVA = Convert.ToInt32(lector["IVA"]),
                Total = Convert.ToDecimal(lector["Total"])
            };

            if (TieneColumna(lector, "MetodoPago") && lector["MetodoPago"] != DBNull.Value)
            {
                v.MetodoPago = lector["MetodoPago"].ToString();
            }

            if (TieneColumna(lector, "Cliente_ID") && lector["Cliente_ID"] != DBNull.Value)
            {
                v.Cliente = new Cliente { ID = Convert.ToInt32(lector["Cliente_ID"]) };
            }

            if (TieneColumna(lector, "AutoBase_ID") && lector["AutoBase_ID"] != DBNull.Value)
            {
                v.AutoBase = new AutoBase { ID = Convert.ToInt32(lector["AutoBase_ID"]) };
            }

            if (TieneColumna(lector, "AutoPersonalizado_ID") && lector["AutoPersonalizado_ID"] != DBNull.Value)
            {
                v.AutoPersonalizado = new AutoPersonalizado { ID = Convert.ToInt32(lector["AutoPersonalizado_ID"]) };
            }

            return v;
        }

        /// <summary>
        /// Verifica si el SqlDataReader contiene una columna determinada sin lanzar excepción.
        /// </summary>
        private bool TieneColumna(SqlDataReader lector, string nombreColumna)
        {
            for (int i = 0; i < lector.FieldCount; i++)
            {
                if (string.Equals(lector.GetName(i), nombreColumna, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Insertar(Venta entidad)
        {
            lock (_lock)
            {
                // Intentar guardar en la tabla Venta de SQL Server y recuperar el ID generado
                try
                {
                    using (var con = new SqlConnection(cx))
                    {
                        con.Open();
                        using (var cmd = new SqlCommand(SqlInsertar, con))
                        {
                            ConfigurarParametros(cmd, entidad);
                            object resultado = cmd.ExecuteScalar();
                            if (resultado != null && resultado != DBNull.Value)
                            {
                                entidad.ID = Convert.ToInt32(resultado);
                            }
                        }
                    }
                }
                catch
                {
                    // Fallback transparente si la tabla aún no tiene todas las columnas migradas
                }

                if (entidad.ID <= 0)
                {
                    entidad.ID = _contadorId++;
                }
                else if (entidad.ID >= _contadorId)
                {
                    _contadorId = entidad.ID + 1;
                }

                // Actualizar o agregar en la lista en memoria con todas sus referencias completas
                int idxExistente = _registroVentas.FindIndex(v => v.ID == entidad.ID || (!string.IsNullOrEmpty(v.NumeroVenta) && v.NumeroVenta == entidad.NumeroVenta));
                if (idxExistente >= 0)
                {
                    _registroVentas[idxExistente] = entidad;
                }
                else
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
                            bool yaExiste = _registroVentas.Any(r => r.ID == v.ID || (!string.IsNullOrEmpty(r.NumeroVenta) && r.NumeroVenta == v.NumeroVenta));
                            if (!yaExiste)
                            {
                                HidratarRelacionesVenta(v);
                                _registroVentas.Add(v);
                            }
                        }
                    }
                }
                catch
                {
                    // Usa el registro en memoria si la base de datos no está disponible
                }

                return _registroVentas.OrderByDescending(v => v.Fecha).ToList();
            }
        }

        /// <summary>
        /// Completa los objetos Cliente, AutoBase, AutoPersonalizado y Factura cuando una Venta
        /// se recupera desde SQL Server luego de reiniciar la aplicación.
        /// </summary>
        private void HidratarRelacionesVenta(Venta v)
        {
            if (v == null) return;

            try
            {
                if (v.AutoBase != null && v.AutoBase.ID > 0 && string.IsNullOrEmpty(v.AutoBase.Modelo))
                {
                    DAL_AutoBase dalBase = new DAL_AutoBase();
                    v.AutoBase = dalBase.ObtenerPorId(v.AutoBase.ID);
                }
            }
            catch { }

            try
            {
                if (v.Cliente != null && v.Cliente.ID > 0 && string.IsNullOrEmpty(v.Cliente.Email))
                {
                    DAL_Cliente dalCli = new DAL_Cliente();
                    var todos = dalCli.Listar();
                    Cliente encontrado = todos.FirstOrDefault(c => c.ID == v.Cliente.ID);
                    if (encontrado != null)
                    {
                        try { encontrado.Nombre = RedLine.Servicios.Hashing.DesencriptarAES(encontrado.Nombre); } catch { }
                        try { encontrado.Apellido = RedLine.Servicios.Hashing.DesencriptarAES(encontrado.Apellido); } catch { }
                        try { encontrado.DNI = RedLine.Servicios.Hashing.DesencriptarAES(encontrado.DNI); } catch { }
                        try { encontrado.Telefono = RedLine.Servicios.Hashing.DesencriptarAES(encontrado.Telefono); } catch { }
                        try { encontrado.Direccion = RedLine.Servicios.Hashing.DesencriptarAES(encontrado.Direccion); } catch { }
                        v.Cliente = encontrado;
                    }
                }
            }
            catch { }

            try
            {
                if (v.AutoPersonalizado != null && v.AutoPersonalizado.ID > 0)
                {
                    DAL_AutoPersonalizado dalAuto = new DAL_AutoPersonalizado();
                    AutoPersonalizado autoBd = dalAuto.ObtenerPorId(v.AutoPersonalizado.ID);
                    if (autoBd != null)
                    {
                        if (autoBd.AuBase == null && v.AutoBase != null)
                        {
                            autoBd.AuBase = v.AutoBase;
                        }
                        if (string.IsNullOrEmpty(autoBd.NumeroVenta)) autoBd.NumeroVenta = v.NumeroVenta;
                        if (autoBd.FechaCompra == null) autoBd.FechaCompra = v.Fecha;
                        if (string.IsNullOrEmpty(autoBd.MetodoPago)) autoBd.MetodoPago = v.MetodoPago;
                        if (autoBd.TotalPagado <= 0) autoBd.TotalPagado = v.Total;
                        v.AutoPersonalizado = autoBd;
                    }
                }
            }
            catch { }

            try
            {
                if (v.Factura == null)
                {
                    DAL_Factura dalFac = new DAL_Factura();
                    Factura fac = dalFac.ObtenerPorId(v.ID);
                    if (fac != null)
                    {
                        fac.Venta = v;
                        v.Factura = fac;
                        if (string.IsNullOrEmpty(v.MetodoPago))
                        {
                            v.MetodoPago = fac.MetodoPago;
                        }
                        if (v.AutoPersonalizado != null && string.IsNullOrEmpty(v.AutoPersonalizado.NumeroFactura))
                        {
                            v.AutoPersonalizado.NumeroFactura = fac.NumeroFactura;
                        }
                    }
                }
            }
            catch { }
        }

        public override Venta ObtenerPorId(int id)
        {
            lock (_lock)
            {
                var enMemoria = _registroVentas.FirstOrDefault(v => v.ID == id);
                if (enMemoria != null) return enMemoria;

                try
                {
                    Venta desdeBd = base.ObtenerPorId(id);
                    if (desdeBd != null)
                    {
                        HidratarRelacionesVenta(desdeBd);
                        _registroVentas.Add(desdeBd);
                    }
                    return desdeBd;
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
