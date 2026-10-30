using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Dal;
using System;

namespace RedLine.BLL
{
    public class BLL_Venta
    {
        private readonly DAL_Venta _dalVenta;

        public BLL_Venta()
        {
            _dalVenta = new DAL_Venta();
        }

        public void CrearVenta(Venta venta, Factura factura)
        {

            if (venta == null)
                throw new ArgumentNullException(nameof(venta), "La venta no puede ser nula.");

            if (venta.Cliente == null)
                throw new InvalidOperationException("Toda venta debe estar asociada a un cliente.");

            if (venta.AutoBase == null && venta.AutoPersonalizado == null)
                throw new InvalidOperationException("La venta debe incluir al menos un vehículo (Base o Personalizado).");

            if (venta.Total <= 0)
                throw new ArgumentException("El total de la venta debe ser mayor a cero.");


            if (venta.Fecha == default)
                venta.Fecha = DateTime.Now;

            if (factura != null)
            {
                if (factura.FechaEmision == default)
                    factura.FechaEmision = venta.Fecha;

                factura.Total = venta.Total;
                factura.IVA = venta.IVA;
            }


            _dalVenta.InsertarVentaConFactura(venta, factura);
        }
    }
}