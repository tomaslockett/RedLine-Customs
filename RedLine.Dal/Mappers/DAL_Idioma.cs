using RedLine.Be.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLine.Dal.Mappers
{
    public class DAL_Idioma : AbstractDAL<int, Idioma>
    {
        protected override string NombreTabla => "Idioma";
        protected override bool RequiereDigitoVerificador => false;

        protected override string SqlInsertar => @"INSERT INTO Idioma (Nombre, EsDefault) VALUES (@Nombre, @EsDefault)";
        protected override string SqlModificar => @"UPDATE Idioma SET Nombre = @Nombre, EsDefault = @EsDefault WHERE ID = @ID";
        protected override string SqlEliminar => @"DELETE FROM Idioma WHERE ID = @ID";
        protected override string SqlListar => @"SELECT ID, Nombre, EsDefault FROM Idioma ORDER BY Nombre";
        protected override string SqlObtenerPorId => @"SELECT ID, Nombre, EsDefault FROM Idioma WHERE ID = @ID";

        protected override void ConfigurarParametros(SqlCommand cmd, Idioma entidad)
        {
            if (cmd.CommandText.Contains("@ID"))
            {
                cmd.Parameters.AddWithValue("@ID", entidad.ID);
            }
            cmd.Parameters.AddWithValue("@Nombre", entidad.Nombre);
            cmd.Parameters.AddWithValue("@EsDefault", entidad.EsDefault);
        }

        protected override void ConfigurarParametrosId(SqlCommand cmd, int id)
        {
            cmd.Parameters.AddWithValue("@ID", id);
        }

        protected override Idioma Mapear(SqlDataReader lector)
        {
            return new Idioma(
                Convert.ToInt32(lector["ID"]),
                lector["Nombre"].ToString(),
                Convert.ToBoolean(lector["EsDefault"])
            );
        }

        public override Idioma ObtenerPorEntidad(Idioma entidad)
        {
            return ObtenerPorId(entidad.ID);
        }

        public override List<Idioma> Listar()
        {
            try
            {
                var lista = base.Listar();
                if (lista != null && lista.Count > 0) return lista;
            }
            catch { }

            return new List<Idioma>
            {
                new Idioma(1, "Español", true),
                new Idioma(2, "Inglés", false)
            };
        }

        public Idioma ObtenerDefault()
        {
            try
            {
                using (var con = new SqlConnection(cx))
                {
                    con.Open();
                    string query = "SELECT TOP 1 ID, Nombre, EsDefault FROM Idioma WHERE EsDefault = 1";
                    using (var cmd = new SqlCommand(query, con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return Mapear(reader);
                    }
                }
            }
            catch { }

            return new Idioma(1, "Español", true);
        }

        public Dictionary<string, string> ObtenerTraduccionesPorIdioma(int idIdioma)
        {
            var traducciones = new Dictionary<string, string>();
            try
            {
                using (var con = new SqlConnection(cx))
                {
                    con.Open();
                    string query = @"
                -- 1. Buscamos el ID del idioma por defecto (Base / Español)
                DECLARE @IdDefault INT = (SELECT TOP 1 ID FROM Idioma WHERE EsDefault = 1);

                -- 2. Traemos la traducción seleccionada; si está vacía o es NULL, caemos al idioma base
                SELECT 
                    E.Clave,
                    COALESCE(
                        NULLIF(T_Elegido.Texto, ''),
                        NULLIF(T_Default.Texto, ''),
                        '[' + E.Clave + ']'
                    ) AS Texto
                FROM Etiqueta E
                LEFT JOIN Traduccion T_Elegido 
                    ON E.ID = T_Elegido.IdEtiqueta AND T_Elegido.IdIdioma = @IdIdioma
                LEFT JOIN Traduccion T_Default 
                    ON E.ID = T_Default.IdEtiqueta AND T_Default.IdIdioma = @IdDefault";

                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdIdioma", idIdioma);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                traducciones[reader["Clave"].ToString()] = reader["Texto"].ToString();
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fallback automático transparente si la BD no está disponible
            }

            // Combinar con traducciones base para garantizar que las nuevas pantallas nunca muestren corchetes
            CargarTraduccionesFallback(traducciones, idIdioma);

            return traducciones;
        }

        private void CargarTraduccionesFallback(Dictionary<string, string> dicc, int idIdioma)
        {
            bool esIngles = idIdioma == 2;

            var fallbacks = new Dictionary<string, string>
            {
                { "lblMenuInicio", esIngles ? "Home" : "Inicio" },
                { "lblMenuTallerVentas", esIngles ? "Workshop & Sales" : "Taller y Ventas" },
                { "lblMenuCrearAuto", esIngles ? "Create Car" : "Crear Auto" },
                { "lblMenuPersonalizarAuto", esIngles ? "Customize Car" : "Personalizar Auto" },
                { "lblMenuInventario", esIngles ? "Inventory" : "Inventario" },
                { "lblMenuHistorialVentas", esIngles ? "Sales History" : "Historial de Ventas" },
                { "lblMenuMisAutos", esIngles ? "My Vehicles" : "Mis Vehículos" },
                { "lblMenuFacturacion", esIngles ? "Invoicing" : "Facturación" },
                { "lblMenuGestionABM", esIngles ? "CRUD Management" : "Gestión ABM" },
                { "lblMenuGestionUsuarios", esIngles ? "User Management" : "Gestión Usuarios" },
                { "lblMenuGestionClientes", esIngles ? "Customer Management" : "Gestión Clientes" },
                { "lblMenuGestionEventos", esIngles ? "Event Log" : "Gestión Eventos" },
                { "lblMenuSistema", esIngles ? "System" : "Sistema" },
                { "lblMenuGestionPermisos", esIngles ? "Permissions Management" : "Gestión de Permisos" },
                { "lblMenuGestionIdiomas", esIngles ? "Language Management" : "Gestión de Idiomas" },
                { "lblMenuBackupRestore", esIngles ? "Backup & Restore" : "Backup & Restore" },
                { "lblMenuDigitoVerificador", esIngles ? "Checksum (DV)" : "Dígito Verificador" },
                { "lblMenuRegistrarse", esIngles ? "Register" : "Registrarse" },
                { "lblMenuLogin", esIngles ? "Sign In" : "Iniciar Sesión" },
                { "lblMenuCambiarPass", esIngles ? "Change Password" : "Cambiar Contraseña" },
                { "lblMenuCerrarSesion", esIngles ? "Log Out" : "Cerrar Sesión" },
                { "litNombreUsuario_Default", esIngles ? "My Account" : "Mi Cuenta" },

                // MisAutos.aspx
                { "lblTituloMiGarage", esIngles ? "My Personal Garage" : "Mi Garage Personal" },
                { "lblSubtituloMiGarage", esIngles ? "Exclusive customer area: view all your purchased vehicles and download their official invoices instantly." : "Sección exclusiva de cliente: aquí puedes visualizar todos tus autos adquiridos y descargar sus facturas oficiales al instante." },
                { "lblTitularBadge", esIngles ? "Owner:" : "Titular:" },
                { "lblDniBadge", esIngles ? "ID / Tax ID:" : "DNI / CUIT:" },
                { "lblTotalAutosBadge", esIngles ? "Total Vehicles Purchased:" : "Total de Autos Adquiridos:" },
                { "lblGarageVacioTitulo", esIngles ? "You don't have vehicles in your personal garage yet" : "Aún no tienes autos en tu garage personal" },
                { "lblGarageVacioTexto", esIngles ? "Explore our exclusive sports fleet in the catalog, customize spoilers, suspension, wheels and bodywork, and complete your purchase at checkout." : "Explora nuestra exclusiva flota deportiva en el catálogo, personaliza alerones, suspensión, llantas y carrocería a tu medida, y completa tu compra en el checkout." },
                { "btnExplorarCatalogo", esIngles ? "Explore Vehicle Catalog" : "Explorar Catálogo de Vehículos" },
                { "lblBadgeEnPropiedad", esIngles ? "Owned" : "En Propiedad" },
                { "lblMejorasInstaladas", esIngles ? "Installed Upgrades:" : "Mejoras Instaladas:" },
                { "lblFacturaNum", esIngles ? "Invoice #:" : "Factura N°:" },
                { "lblVentaNum", esIngles ? "Sale #:" : "Venta N°:" },
                { "lblFechaCompra", esIngles ? "Date:" : "Fecha:" },
                { "lblMetodoPagoCard", esIngles ? "Payment Method:" : "Método de Pago:" },
                { "lblMontoAbonado", esIngles ? "Amount Paid:" : "Monto Abonado:" },
                { "lblBtnDescargarPdf", esIngles ? "Download Invoice (PDF)" : "Descargar Factura (PDF)" },
                { "btnDescargarPdf", esIngles ? "Download Invoice (PDF)" : "Descargar Factura (PDF)" },
                { "lblBtnVerFactura", esIngles ? "View Invoice" : "Ver Factura" },
                { "btnVerFactura", esIngles ? "View Invoice" : "Ver Factura" },
                { "texto_configuracion_fabrica", esIngles ? "Factory Stock Setup" : "Configuración de Fábrica" },
                { "msg_factura_no_encontrada", esIngles ? "The requested invoice was not found." : "No se encontró el comprobante de factura solicitado." },

                // Facturacion.aspx
                { "lblFacturaVaciaTitulo", esIngles ? "No invoices issued at this time" : "No se registran comprobantes emitidos por el momento" },
                { "lblFacturaVaciaTexto", esIngles ? "Your official electronic invoices will appear here automatically upon confirming the purchase of a vehicle in our workshop." : "Tus facturas electrónicas oficiales aparecerán aquí automáticamente al confirmar la adquisición de un vehículo en nuestro taller." },
                { "btnIrCatalogo", esIngles ? "Explore Fleet in Catalog" : "Explorar Flota en Catálogo" },
                { "btnIrMisAutos", esIngles ? "Go to My Garage" : "Ir a Mi Garage" },
                { "lblTituloFacturacion", esIngles ? "Electronic Invoice Issued" : "Comprobante Electrónico Emitido" },
                { "lblSubtituloFacturacion", esIngles ? "Invoice valid as tax receipt and official warranty." : "Factura válida como constancia fiscal y garantía oficial." },
                { "lblBtnImprimir", esIngles ? "Print" : "Imprimir" },
                { "lblBtnVerMisAutos", esIngles ? "My Purchased Vehicles" : "Mis Vehículos Comprados" },
                { "lblRazonSocialEmisor", esIngles ? "Company Name:" : "Razón Social:" },
                { "lblCuitEmisor", esIngles ? "Tax ID:" : "CUIT:" },
                { "lblIvaEmisor", esIngles ? "VAT:" : "IVA:" },
                { "lblIngresosBrutosEmisor", esIngles ? "Gross Income Tax:" : "Ingresos Brutos:" },
                { "lblDomicilioEmisor", esIngles ? "Address:" : "Domicilio:" },
                { "lblInicioActividadesEmisor", esIngles ? "Start of Activities:" : "Inicio de Actividades:" },
                { "lblCodigoComprobante", "COD. 06" },
                { "lblFacturaElectronica", esIngles ? "ELECTRONIC INVOICE" : "FACTURA ELECTRÓNICA" },
                { "lblTextoFechaEmision", esIngles ? "Issue Date:" : "Fecha de Emisión:" },
                { "lblTextoNumeroVenta", esIngles ? "Sale Number:" : "Número de Venta:" },
                { "lblTextoCondicionVenta", esIngles ? "Sale Condition:" : "Condición de Venta:" },
                { "lblTextoMetodoPago", esIngles ? "Payment Method:" : "Método de Pago:" },
                { "lblTextoNombreCliente", esIngles ? "Full Name / Company Name:" : "Nombre y Apellido / Razón Social:" },
                { "lblTextoDniCliente", esIngles ? "ID / Tax ID:" : "DNI / CUIT:" },
                { "lblTextoEmailCliente", esIngles ? "Email:" : "Correo Electrónico:" },
                { "lblTextoCondicionIvaCliente", esIngles ? "VAT Category:" : "Condición frente al IVA:" },
                { "lblColConcepto", esIngles ? "Description / Item" : "Descripción / Concepto" },
                { "lblColCantidad", esIngles ? "Qty." : "Cant." },
                { "lblColPrecioUnitario", esIngles ? "Unit Price" : "Precio Unitario" },
                { "lblColSubtotal", esIngles ? "Subtotal" : "Subtotal" },
                { "lblTextoSubtotalNeto", esIngles ? "Net Subtotal:" : "Subtotal Neto:" },
                { "lblTextoIva21", esIngles ? "VAT (21%):" : "IVA Liquidado (21%):" },
                { "lblTextoTotalFactura", esIngles ? "TOTAL INVOICED:" : "TOTAL FACTURADO:" },
                { "lblTextoCae", esIngles ? "CAE #:" : "CAE N°:" },
                { "lblTextoVtoCae", esIngles ? "CAE Expiration Date:" : "Fecha de Vto. de CAE:" }
            };

            foreach (var kvp in fallbacks)
            {
                if (!dicc.ContainsKey(kvp.Key) || string.IsNullOrEmpty(dicc[kvp.Key]) || dicc[kvp.Key].StartsWith("["))
                {
                    dicc[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}
