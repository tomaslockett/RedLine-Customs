using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Dal;
using RedLine.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedLine.Bll
{
    public class BLL_Venta : AbstractBLL<int, Venta>
    {
        private readonly DAL_Venta _dalVenta;
        private readonly DAL_Factura _dalFactura;
        private readonly BLL_Evento _bllEvento;

        public BLL_Venta() : base(new DAL_Venta())
        {
            _dalVenta = (DAL_Venta)_repositorio;
            _dalFactura = new DAL_Factura();
            _bllEvento = new BLL_Evento();
        }

        public Venta RegistrarVenta(Cliente cliente, AutoBase autoBase, string extrasCsv, string metodoPago, string referenciaPago = "")
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "Se requiere un cliente para registrar la venta.");
            }

            if (autoBase == null)
            {
                throw new ArgumentNullException(nameof(autoBase), "Se requiere un vehículo base para concretar la compra.");
            }

            // Mapeo de extras y cálculo de importes
            var preciosExtras = new Dictionary<string, (string Nombre, decimal Precio, string Categoria)>
            {
                { "aleron", ("Alerón deportivo", 500m, "Aleron") },
                { "kitCarroceria", ("Kit de carrocería", 1200m, "Kit") },
                { "llantas", ("Llantas personalizadas", 2500m, "Llantas") },
                { "suspension", ("Suspensión ajustable", 3000m, "Suspension") },
                { "pintura", ("Pintura personalizada", 1800m, "Pintura") }
            };

            decimal subtotal = autoBase.PrecioBase;
            var mejoras = new List<Mejora>();
            int indexMejora = 1;

            if (!string.IsNullOrEmpty(extrasCsv))
            {
                string[] arrayExtras = extrasCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string key in arrayExtras)
                {
                    string limpia = key.Trim();
                    if (preciosExtras.ContainsKey(limpia))
                    {
                        var data = preciosExtras[limpia];
                        subtotal += data.Precio;

                        Mejora m;
                        switch (data.Categoria)
                        {
                            case "Aleron": m = new Aleron(); break;
                            case "Kit": m = new KitCarroceria(); break;
                            case "Llantas": m = new Llantas(); break;
                            case "Suspension": m = new Suspension(); break;
                            case "Pintura": m = new Pintura(); break;
                            default: m = new Aleron(); break;
                        }

                        m.ID = indexMejora++;
                        m.Nombre = data.Nombre;
                        m.Precio = data.Precio;
                        m.Stock = 10;
                        m.Categoria = data.Categoria;

                        mejoras.Add(m);
                    }
                }
            }

            decimal iva = Math.Round(subtotal * 0.21m, 2);
            decimal total = subtotal + iva;

            string marcaTiempo = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string numVenta = "VTA-" + marcaTiempo;
            string numFactura = "FC-B-0001-" + DateTime.Now.ToString("ffffss");

            // Crear el Auto Personalizado que va al Garage del Cliente
            AutoPersonalizado autoComprado = new AutoPersonalizado
            {
                ID = new Random().Next(100, 99999),
                DNI_Cliente = cliente.DNI,
                AuBase = autoBase,
                Mejoras = mejoras,
                NombreExtra = mejoras.Count > 0 ? string.Join(", ", mejoras.Select(m => m.Nombre)) : "Configuración Estándar",
                FechaCompra = DateTime.Now,
                NumeroVenta = numVenta,
                NumeroFactura = numFactura,
                MetodoPago = metodoPago,
                TotalPagado = total
            };

            // Crear la Venta
            Venta venta = new Venta
            {
                NumeroVenta = numVenta,
                Cliente = cliente,
                AutoBase = autoBase,
                AutoPersonalizado = autoComprado,
                Fecha = DateTime.Now,
                IVA = 21,
                Total = total,
                MetodoPago = metodoPago
            };

            // Crear la Factura asociada
            Factura factura = new Factura
            {
                NumeroFactura = numFactura,
                FechaEmision = DateTime.Now,
                Total = total,
                MetodoPago = metodoPago,
                IVA = 21,
                Venta = venta
            };

            venta.Factura = factura;

            // Asociar al garage del cliente
            if (cliente.Garage == null)
            {
                cliente.Garage = new List<AutoPersonalizado>();
            }
            cliente.Garage.Add(autoComprado);

            // Persistir Venta y Factura
            _dalVenta.Insertar(venta);
            _dalFactura.Insertar(factura);

            // Registrar en auditoría / bitácora
            try
            {
                string usuarioOperacion = SessionManager.Instancia.IsLogged() ? SessionManager.Instancia.Usuario.Email : cliente.Email;
                _bllEvento.Registrar(
                    usuarioOperacion,
                    ModulosEventos.Taller,
                    $"Venta #{numVenta} concretada. Vehículo: {autoBase.Marca} {autoBase.Modelo} por un total de ${total:N0}. Factura: {numFactura}.",
                    1
                );
            }
            catch
            {
            }

            return venta;
        }

        public List<Venta> ListarPorCliente(string dniOEmail)
        {
            if (string.IsNullOrEmpty(dniOEmail)) return new List<Venta>();

            return Listar().Where(v =>
                (v.Cliente != null && (
                    string.Equals(v.Cliente.DNI, dniOEmail, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(v.Cliente.Email, dniOEmail, StringComparison.OrdinalIgnoreCase)
                )) ||
                (v.AutoPersonalizado != null && string.Equals(v.AutoPersonalizado.DNI_Cliente, dniOEmail, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        public Venta ObtenerPorNumero(string numeroVenta)
        {
            return _dalVenta.ObtenerPorNumero(numeroVenta);
        }

        public Venta ObtenerPorNumeroFactura(string numeroFactura)
        {
            return Listar().FirstOrDefault(v => v.Factura != null && string.Equals(v.Factura.NumeroFactura, numeroFactura, StringComparison.OrdinalIgnoreCase));
        }
    }
}
