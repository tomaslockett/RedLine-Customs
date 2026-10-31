<%@ Page Title="Facturación Oficial" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Facturacion.aspx.cs" Inherits="RedLine.Web.Facturacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />
    <link rel="stylesheet" href="Content/Facturacion.css" />

    <main class="factura-wrapper">

        <!-- Estado Vacío: Si no hay facturas emitidas todavía -->
        <asp:PlaceHolder ID="phFacturaVacia" runat="server" Visible="false">
            <div class="factura-vacia">
                <div class="factura-vacia-icon">
                    <i class="fa-solid fa-file-invoice-dollar"></i>
                </div>
                <h2>
                    <asp:Label ID="lblFacturaVaciaTitulo" runat="server" Text="No se registran comprobantes emitidos por el momento" />
                </h2>
                <p>
                    <asp:Label ID="lblFacturaVaciaTexto" runat="server" Text="Tus facturas electrónicas oficiales aparecerán aquí automáticamente al confirmar la adquisición de un vehículo en nuestro taller." />
                </p>
                <div class="factura-vacia-acciones">
                    <a href="Catalogo.aspx" class="btn-factura-pdf">
                        <i class="fa-solid fa-car"></i> <asp:Label ID="btnIrCatalogo" runat="server" Text="Explorar Flota en Catálogo" />
                    </a>
                    <a href="MiGaraje.aspx" class="btn-factura-print">
                        <i class="fa-solid fa-warehouse"></i> <asp:Label ID="btnIrMisAutos" runat="server" Text="Ir a Mi Garage" />
                    </a>
                </div>
            </div>
        </asp:PlaceHolder>

        <!-- Detalle de Factura Emitida -->
        <asp:PlaceHolder ID="phFacturaDetalle" runat="server" Visible="true">
            <!-- Toolbar de Acciones (Descargar PDF, Imprimir, Selector de Comprobante) -->
            <div class="factura-toolbar">
                <div class="toolbar-info">
                    <i class="fa-solid fa-file-invoice-dollar fa-xl"></i>
                    <div>
                        <strong><asp:Label ID="lblTituloFacturacion" runat="server" Text="Comprobante Electrónico Emitido" /></strong>
                        <div class="toolbar-subtitulo">
                            <asp:Label ID="lblSubtituloFacturacion" runat="server" Text="Factura válida como constancia fiscal y garantía oficial." />
                        </div>
                    </div>
                </div>

                <div class="toolbar-botones">
                    <asp:DropDownList ID="ddlFacturas" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFacturas_SelectedIndexChanged" 
                        CssClass="btn-factura-print select-facturas" Visible="false">
                    </asp:DropDownList>

                    <asp:LinkButton ID="btnDescargarPdf" runat="server" CssClass="btn-factura-pdf" OnClick="btnDescargarPdf_Click">
                        <i class="fa-solid fa-file-pdf"></i> <asp:Label ID="lblFactBtnDescargarPdf" runat="server" Text="Descargar Factura (PDF)" />
                    </asp:LinkButton>

                    <button type="button" class="btn-factura-print" onclick="window.print();">
                        <i class="fa-solid fa-print"></i> <asp:Label ID="lblBtnImprimir" runat="server" Text="Imprimir" />
                    </button>

                    <a href="MiGaraje.aspx" class="btn-factura-print">
                        <i class="fa-solid fa-warehouse"></i> <asp:Label ID="lblBtnVerMisAutos" runat="server" Text="Mis Vehículos Comprados" />
                    </a>
                </div>
            </div>

            <!-- Hoja de Factura Comercial Oficial -->
            <section class="hoja-factura">
                <header class="factura-encabezado">
                    <!-- Columna Izquierda: Emisor -->
                    <div class="emisor-info">
                        <h2>REDLINE CUSTOMS</h2>
                        <p class="emisor-sub"><asp:Label ID="lblSubtituloEmisor" runat="server" Text="Modificaciones & Concesionaria Oficial" /></p>
                        <div class="emisor-detalles">
                            <strong><asp:Label ID="lblRazonSocialEmisor" runat="server" Text="Razón Social:" /></strong> RedLine Customs S.A.<br />
                            <strong><asp:Label ID="lblCuitEmisor" runat="server" Text="CUIT:" /></strong> 30-71829384-9<br />
                            <strong><asp:Label ID="lblIvaEmisor" runat="server" Text="IVA:" /></strong> <asp:Label ID="lblValorIvaEmisor" runat="server" Text="Responsable Inscripto" /><br />
                            <strong><asp:Label ID="lblIngresosBrutosEmisor" runat="server" Text="Ingresos Brutos:" /></strong> 30-71829384-9<br />
                            <strong><asp:Label ID="lblDomicilioEmisor" runat="server" Text="Domicilio:" /></strong> Av. del Libertador 4500, CABA<br />
                            <strong><asp:Label ID="lblInicioActividadesEmisor" runat="server" Text="Inicio de Actividades:" /></strong> 01/03/2020
                        </div>
                    </div>

                    <!-- Centro: Tipo de Comprobante -->
                    <div class="tipo-comprobante-box">
                        <span class="tipo-letra">B</span>
                        <span class="tipo-codigo"><asp:Label ID="lblCodigoComprobante" runat="server" Text="COD. 06" /></span>
                    </div>

                    <!-- Columna Derecha: Datos de la Factura -->
                    <div class="comprobante-info">
                        <h3><asp:Label ID="lblFacturaElectronica" runat="server" Text="FACTURA ELECTRÓNICA" /></h3>
                        <div class="comprobante-numero">
                            <asp:Label ID="lblNumeroFactura" runat="server" Text="FC-B-0001-00000000" />
                        </div>
                        <div class="comprobante-detalles">
                            <strong><asp:Label ID="lblTextoFechaEmision" runat="server" Text="Fecha de Emisión:" /></strong> <asp:Label ID="lblFechaEmision" runat="server" Text="--" /><br />
                            <strong><asp:Label ID="lblTextoNumeroVenta" runat="server" Text="Número de Venta:" /></strong> <asp:Label ID="lblNumeroVenta" runat="server" Text="--" /><br />
                            <strong><asp:Label ID="lblTextoCondicionVenta" runat="server" Text="Condición de Venta:" /></strong> <asp:Label ID="lblValorCondicionVenta" runat="server" Text="Contado Inmediato" /><br />
                            <strong><asp:Label ID="lblTextoMetodoPago" runat="server" Text="Método de Pago:" /></strong> <asp:Label ID="lblMetodoPago" runat="server" Text="Tarjeta" />
                        </div>
                    </div>
                </header>

                <!-- Datos del Cliente / Receptor -->
                <section class="factura-cliente-seccion">
                    <div class="cliente-dato">
                        <span><asp:Label ID="lblTextoNombreCliente" runat="server" Text="Nombre y Apellido / Razón Social:" /></span>
                        <span><asp:Label ID="lblNombreCliente" runat="server" Text="Consumidor Final" /></span>
                    </div>
                    <div class="cliente-dato">
                        <span><asp:Label ID="lblTextoDniCliente" runat="server" Text="DNI / CUIT:" /></span>
                        <span><asp:Label ID="lblDniCliente" runat="server" Text="--" /></span>
                    </div>
                    <div class="cliente-dato">
                        <span><asp:Label ID="lblTextoEmailCliente" runat="server" Text="Correo Electrónico:" /></span>
                        <span><asp:Label ID="lblEmailCliente" runat="server" Text="--" /></span>
                    </div>
                    <div class="cliente-dato">
                        <span><asp:Label ID="lblTextoCondicionIvaCliente" runat="server" Text="Condición frente al IVA:" /></span>
                        <span><asp:Label ID="lblValorCondicionIvaCliente" runat="server" Text="Consumidor Final" /></span>
                    </div>
                </section>

                <!-- Tabla de Ítems / Conceptos Facturados -->
                <div class="tabla-items-responsive">
                    <table class="tabla-items-factura">
                        <thead>
                            <tr>
                                <th class="col-concepto"><asp:Label ID="lblColConcepto" runat="server" Text="Descripción / Concepto" /></th>
                                <th class="col-cantidad"><asp:Label ID="lblColCantidad" runat="server" Text="Cant." /></th>
                                <th class="col-precio-unitario"><asp:Label ID="lblColPrecioUnitario" runat="server" Text="Precio Unitario" /></th>
                                <th class="col-subtotal"><asp:Label ID="lblColSubtotal" runat="server" Text="Subtotal" /></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="repItemsFactura" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><strong><%# Eval("Concepto") %></strong></td>
                                        <td><%# Eval("Cantidad") %></td>
                                        <td>$<%# Eval("PrecioUnitario", "{0:N0}") %></td>
                                        <td><strong>$<%# Eval("Subtotal", "{0:N0}") %></strong></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>

                <!-- Resumen Financiero y Totales -->
                <section class="factura-totales-wrapper">
                    <table class="totales-tabla">
                        <tr>
                            <td><asp:Label ID="lblTextoSubtotalNeto" runat="server" Text="Subtotal Neto:" /></td>
                            <td><asp:Label ID="lblSubtotalNeto" runat="server" Text="$0" /></td>
                        </tr>
                        <tr>
                            <td><asp:Label ID="lblTextoIva21" runat="server" Text="IVA Liquidado (21%):" /></td>
                            <td><asp:Label ID="lblIvaNeto" runat="server" Text="$0" /></td>
                        </tr>
                        <tr class="total-destacado-fila">
                            <td><asp:Label ID="lblTextoTotalFactura" runat="server" Text="TOTAL FACTURADO:" /></td>
                            <td><asp:Label ID="lblTotalFacturado" runat="server" Text="$0" /></td>
                        </tr>
                    </table>
                </section>

                <!-- Pie Fiscal Oficial con CAE y QR Mock -->
                <footer class="factura-pie-fiscal">
                    <div class="cae-info">
                        <strong><asp:Label ID="lblTextoCae" runat="server" Text="CAE N°:" /></strong> 74928301928374<br />
                        <strong><asp:Label ID="lblTextoVtoCae" runat="server" Text="Fecha de Vto. de CAE:" /></strong> <asp:Label ID="lblVencimientoCae" runat="server" Text="--" /><br />
                        <span><asp:Label ID="lblLeyendaPieFactura" runat="server" Text="Comprobante Electrónico emitido por RedLine Customs. Válido como garantía de taller." /></span>
                    </div>
                    <div class="cae-qr-mock">
                        <i class="fa-solid fa-qrcode fa-2xl"></i>
                    </div>
                </footer>
            </section>
        </asp:PlaceHolder>

    </main>
</asp:Content>