using RedLine.Be.Entidades;
using RedLine.Dal.Mappers;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Bll
{
    public class BLL_Factura : AbstractBLL<int, Factura>
    {
        private readonly DAL_Factura _dalFactura;

        public BLL_Factura() : base(new DAL_Factura())
        {
            _dalFactura = (DAL_Factura)_repositorio;
        }

        public Factura ObtenerPorNumero(string numeroFactura)
        {
            return _dalFactura.ObtenerPorNumero(numeroFactura);
        }

        public List<Factura> ListarPorCliente(string dniOEmail)
        {
            if (string.IsNullOrEmpty(dniOEmail)) return new List<Factura>();

            return Listar().Where(f =>
                f.Venta != null && f.Venta.Cliente != null && (
                    string.Equals(f.Venta.Cliente.DNI, dniOEmail, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(f.Venta.Cliente.Email, dniOEmail, StringComparison.OrdinalIgnoreCase)
                )
            ).ToList();
        }

        public byte[] GenerarPdf(Factura factura)
        {
            if (factura == null) throw new ArgumentNullException(nameof(factura));
            return PdfFacturaServicio.GenerarPdfFactura(factura);
        }
    }
}
