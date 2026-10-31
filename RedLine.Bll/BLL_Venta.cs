using Redline.Be;
using RedLine.Be.Entidades;
using RedLine.Dal.Mappers;
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

        /// <summary>
        /// Registra en el sistema una Venta y su Factura asociada ya construidas desde la capa de presentación (Checkout).
        /// Enlaza ambas entidades entre sí, las persiste y registra el evento en la bitácora.
        /// </summary>
        public void CrearVenta(Venta nuevaVenta, Factura nuevaFactura)
        {
            if (nuevaVenta == null)
            {
                throw new ArgumentNullException(nameof(nuevaVenta), "Los datos de la venta no pueden ser nulos.");
            }

            if (nuevaFactura == null)
            {
                throw new ArgumentNullException(nameof(nuevaFactura), "Los datos de la factura no pueden ser nulos.");
            }

            // Asegurar referencias cruzadas entre Venta, Factura, AutoBase y AutoPersonalizado
            nuevaVenta.Factura = nuevaFactura;
            nuevaFactura.Venta = nuevaVenta;

            if (nuevaVenta.AutoBase == null && nuevaVenta.AutoPersonalizado != null)
            {
                nuevaVenta.AutoBase = nuevaVenta.AutoPersonalizado.AuBase;
            }

            if (string.IsNullOrEmpty(nuevaVenta.MetodoPago))
            {
                nuevaVenta.MetodoPago = nuevaFactura.MetodoPago;
            }

            if (nuevaVenta.AutoPersonalizado != null)
            {
                nuevaVenta.AutoPersonalizado.NumeroVenta = nuevaVenta.NumeroVenta;
                nuevaVenta.AutoPersonalizado.NumeroFactura = nuevaFactura.NumeroFactura;
                nuevaVenta.AutoPersonalizado.FechaCompra = nuevaVenta.Fecha;
                nuevaVenta.AutoPersonalizado.MetodoPago = nuevaVenta.MetodoPago;
                nuevaVenta.AutoPersonalizado.TotalPagado = nuevaVenta.Total;
            }

            // Asociar al garage en memoria del cliente si corresponde
            if (nuevaVenta.Cliente != null && nuevaVenta.AutoPersonalizado != null)
            {
                if (nuevaVenta.Cliente.Garage == null)
                {
                    nuevaVenta.Cliente.Garage = new List<AutoPersonalizado>();
                }

                bool yaEnGarage = false;
                foreach (AutoPersonalizado a in nuevaVenta.Cliente.Garage)
                {
                    if (a.ID == nuevaVenta.AutoPersonalizado.ID && a.NumeroVenta == nuevaVenta.NumeroVenta)
                    {
                        yaEnGarage = true;
                        break;
                    }
                }

                if (!yaEnGarage)
                {
                    nuevaVenta.Cliente.Garage.Add(nuevaVenta.AutoPersonalizado);
                }
            }

            // Guardar Venta y Factura
            _dalVenta.Insertar(nuevaVenta);
            nuevaFactura.ID = nuevaVenta.ID;
            _dalFactura.Insertar(nuevaFactura);

            // Registrar en bitácora sin interrumpir la compra si falla el log
            try
            {
                string emailUsuario = "Cliente";
                if (SessionManager.Instancia.IsLogged() && SessionManager.Instancia.Usuario != null)
                {
                    emailUsuario = SessionManager.Instancia.Usuario.Email;
                }
                else if (nuevaVenta.Cliente != null && !string.IsNullOrEmpty(nuevaVenta.Cliente.Email))
                {
                    emailUsuario = nuevaVenta.Cliente.Email;
                }

                string vehiculoDesc = nuevaVenta.AutoBase != null
                    ? (nuevaVenta.AutoBase.Marca + " " + nuevaVenta.AutoBase.Modelo)
                    : (nuevaVenta.AutoPersonalizado != null ? (nuevaVenta.AutoPersonalizado.Marca + " " + nuevaVenta.AutoPersonalizado.Modelo) : "Vehículo");

                _bllEvento.Registrar(
                    emailUsuario,
                    ModulosEventos.Taller,
                    $"Venta #{nuevaVenta.NumeroVenta} concretada. Vehículo: {vehiculoDesc} por un total de ${nuevaVenta.Total:N2}. Factura: {nuevaFactura.NumeroFactura}.",
                    1
                );
            }
            catch
            {
            }
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

            // Mapeo de extras y cálculo de importes usando la clase unificada Mejora
            var preciosExtras = new Dictionary<string, (string Nombre, decimal Precio, string Categoria)>
            {
                { "aleron", ("Alerón Deportivo", 500m, "Aleron") },
                { "kitCarroceria", ("Kit de Carrocería", 1200m, "Kit") },
                { "llantas", ("Llantas de Aleación", 2500m, "Llantas") },
                { "suspension", ("Suspensión Deportiva", 3000m, "Suspension") },
                { "pintura", ("Pintura Especial", 1800m, "Pintura") }
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

                        Mejora m = new Mejora
                        {
                            ID = indexMejora++,
                            Nombre = data.Nombre,
                            Precio = data.Precio,
                            Stock = 10,
                            Categoria = data.Categoria
                        };

                        mejoras.Add(m);
                    }
                }
            }

            decimal iva = Math.Round(subtotal * 0.21m, 2);
            decimal total = subtotal + iva;

            string marcaTiempo = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string numVenta = "VTA-" + marcaTiempo;
            string numFactura = "FC-B-0001-" + DateTime.Now.ToString("ffffss");

            AutoPersonalizado autoComprado = new AutoPersonalizado
            {
                ID = new Random().Next(100, 99999),
                DNI_Cliente = cliente.DNI,
                Cliente = cliente,
                AuBase = autoBase,
                Mejoras = mejoras,
                NombreExtra = mejoras.Count > 0 ? string.Join(", ", mejoras.Select(m => m.Nombre)) : "Configuración Estándar",
                FechaCompra = DateTime.Now,
                NumeroVenta = numVenta,
                NumeroFactura = numFactura,
                MetodoPago = metodoPago,
                TotalPagado = total
            };

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

            Factura factura = new Factura
            {
                NumeroFactura = numFactura,
                FechaEmision = DateTime.Now,
                Total = total,
                MetodoPago = metodoPago,
                IVA = 21,
                Venta = venta
            };

            CrearVenta(venta, factura);

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
