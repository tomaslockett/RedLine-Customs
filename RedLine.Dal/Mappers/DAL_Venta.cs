using Redline.Be;
using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RedLine.Dal
{
    public class DAL_Venta : AbstractDAL<int, Venta>
    {
        protected override string NombreTabla => "Venta";
        protected override bool RequiereDigitoVerificador => true;

        protected override string SqlInsertar =>
            @"INSERT INTO Venta (NumeroVenta, Fecha, IVA, Total, AutoBase_ID, AutoPersonalizado_ID, Cliente_ID)
              VALUES (@NumeroVenta, @Fecha, @IVA, @Total, @AutoBase_ID, @AutoPersonalizado_ID, @Cliente_ID);
              SELECT SCOPE_IDENTITY();"; 

        protected override string SqlModificar => throw new NotSupportedException("Las ventas no se pueden modificar por motivos de auditoría e integridad.");
        protected override string SqlEliminar => throw new NotSupportedException("Las ventas no se pueden eliminar.");
        protected override string SqlListar => "SELECT * FROM Venta";
        protected override string SqlObtenerPorId => "SELECT * FROM Venta WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Venta entidad)
        {
            if (entidad.ID > 0)
                cmd.Parameters.AddWithValue("@ID", entidad.ID);

            cmd.Parameters.AddWithValue("@NumeroVenta", (object)entidad.NumeroVenta ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fecha", entidad.Fecha);
            cmd.Parameters.AddWithValue("@IVA", entidad.IVA);
            cmd.Parameters.AddWithValue("@Total", entidad.Total);

            cmd.Parameters.AddWithValue("@AutoBase_ID", entidad.AutoBase != null ? (object)entidad.AutoBase.ID : DBNull.Value);
            cmd.Parameters.AddWithValue("@AutoPersonalizado_ID", entidad.AutoPersonalizado != null ? (object)entidad.AutoPersonalizado.ID : DBNull.Value);
            cmd.Parameters.AddWithValue("@Cliente_ID", entidad.Cliente != null ? (object)entidad.Cliente.ID : DBNull.Value);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Venta Mapear(SqlDataReader lector)
        {
            var venta = new Venta
            {
                ID = Convert.ToInt32(lector["ID"]),
                NumeroVenta = lector["NumeroVenta"] != DBNull.Value ? lector["NumeroVenta"].ToString() : null,
                Fecha = Convert.ToDateTime(lector["Fecha"]),
                IVA = Convert.ToInt32(lector["IVA"]),
                Total = Convert.ToDecimal(lector["Total"])
            };

            // Recupera los ID, pero no toda la entidad completa
            if (lector["AutoBase_ID"] != DBNull.Value)
            {
                venta.AutoBase = new AutoBase { ID = Convert.ToInt32(lector["AutoBase_ID"]) };
            }

            if (lector["AutoPersonalizado_ID"] != DBNull.Value)
            {
                venta.AutoPersonalizado = new AutoPersonalizado { ID = Convert.ToInt32(lector["AutoPersonalizado_ID"]) };
            }

            if (lector["Cliente_ID"] != DBNull.Value)
            {
                venta.Cliente = new Cliente { ID = Convert.ToInt32(lector["Cliente_ID"]) };
            }

            return venta;
        }

        public override Venta ObtenerPorEntidad(Venta entidad) => ObtenerPorId(entidad.ID);


        public void InsertarVentaConFactura(Venta venta, Factura factura)
        {
            using (var con = new SqlConnection(cx))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {

                        int ventaId;
                        using (var cmdVenta = new SqlCommand(SqlInsertar, con, tra))
                        {
                            ConfigurarParametros(cmdVenta, venta);
                            ventaId = Convert.ToInt32(cmdVenta.ExecuteScalar());
                            venta.ID = ventaId;
                        }

                       
                        if (factura != null)
                        {
                            string sqlFactura = @"INSERT INTO Factura (ID, NumeroFactura, FechaEmision, Total, MetodoPago, IVA)
                                                  VALUES (@ID, @NumeroFactura, @FechaEmision, @Total, @MetodoPago, @IVA)";

                            using (var cmdFactura = new SqlCommand(sqlFactura, con, tra))
                            {
                                cmdFactura.Parameters.AddWithValue("@ID", ventaId);
                                cmdFactura.Parameters.AddWithValue("@NumeroFactura", (object)factura.NumeroFactura ?? DBNull.Value);
                                cmdFactura.Parameters.AddWithValue("@FechaEmision", factura.FechaEmision);
                                cmdFactura.Parameters.AddWithValue("@Total", factura.Total);
                                cmdFactura.Parameters.AddWithValue("@MetodoPago", (object)factura.MetodoPago ?? DBNull.Value);
                                cmdFactura.Parameters.AddWithValue("@IVA", factura.IVA);
                                cmdFactura.ExecuteNonQuery();
                            }
                            factura.ID = ventaId;
                            venta.Factura = factura;
                        }

                      
                        if (RequiereDigitoVerificador)
                        {
                            this.RecalcularMisDigitosVerificadores(con, tra);
                        }

                        tra.Commit();
                    }
                    catch (Exception)
                    {
                        tra.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}