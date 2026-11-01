namespace RedLine.Servicios
{
    /// <summary>
    /// Cadenas canónicas que Checkout persiste en la base de datos (AutoPersonalizado.NombreExtra,
    /// MetodoPago de Venta/Factura). Son centinelas de datos, NO textos de UI: no se traducen.
    /// Todo escritor (Checkout) y todo lector-comparador (Facturación, HistorialVentas, MiGaraje)
    /// deben referenciar estas constantes para no divergir si el contrato cambia.
    /// El texto mostrable traducido del método de pago se obtiene con la clave
    /// "texto_metodo_pago_tarjeta" en Facturación.
    /// </summary>
    public static class TextosComercialesVentas
    {
        public const string MetodoPagoTarjeta = "Tarjeta de Crédito";
        public const string ConfiguracionEstandar = "Configuración Estándar";
    }
}
