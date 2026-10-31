<%@ Page Title="Mis Vehículos Comprados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MiGaraje.aspx.cs" Inherits="RedLine.Web.MiGaraje" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />
    <link href="Content/MiGaraje.css" rel="stylesheet" type="text/css" />

    <main class="misautos-container">
        <!-- Encabezado de la página del cliente -->
        <header class="misautos-header">
            <h1><i class="fa-solid fa-warehouse icono-titulo-garage"></i><asp:Label ID="lblTituloMiGarage" runat="server" Text="Mi Garage Personal" /></h1>
            <p><asp:Label ID="lblSubtituloMiGarage" runat="server" Text="Sección exclusiva de cliente: aquí puedes visualizar todos tus autos adquiridos y descargar sus facturas oficiales al instante." /></p>
        </header>

        <!-- Barra informativa del cliente -->
        <div class="cliente-badge-bar">
            <div class="cliente-badge-item">
                <i class="fa-solid fa-user-check"></i>
                <span><asp:Label ID="lblTitularBadge" runat="server" Text="Titular:" /> <asp:Label ID="lblNombreCliente" runat="server" Font-Bold="true" Text="Cliente" /></span>
            </div>
            <div class="cliente-badge-item">
                <i class="fa-solid fa-id-card"></i>
                <span><asp:Label ID="lblDniBadge" runat="server" Text="DNI / CUIT:" /> <asp:Label ID="lblDniCliente" runat="server" Font-Bold="true" Text="-" /></span>
            </div>
            <div class="cliente-badge-item">
                <i class="fa-solid fa-car-side"></i>
                <span><asp:Label ID="lblTotalAutosBadge" runat="server" Text="Total de Autos Adquiridos:" /> <asp:Label ID="lblCantidadAutos" runat="server" Font-Bold="true" Text="0" /></span>
            </div>
        </div>

        <!-- Estado Vacío: Si no ha comprado autos aún -->
        <asp:PlaceHolder ID="phGarageVacio" runat="server" Visible="false">
            <div class="garage-vacio">
                <div class="garage-vacio-icon">
                    <i class="fa-solid fa-car-tunnel"></i>
                </div>
                <h2><asp:Label ID="lblGarageVacioTitulo" runat="server" Text="Aún no tienes autos en tu garage personal" /></h2>
                <p><asp:Label ID="lblGarageVacioTexto" runat="server" Text="Explora nuestra exclusiva flota deportiva en el catálogo, personaliza alerones, suspensión, llantas y carrocería a tu medida, y completa tu compra en el checkout." /></p>
                <a href="Catalogo.aspx" class="btn-descargar-pdf btn-explorar-vacio">
                    <i class="fa-solid fa-magnifying-glass"></i> <asp:Label ID="btnExplorarCatalogo" runat="server" Text="Explorar Catálogo de Vehículos" />
                </a>
            </div>
        </asp:PlaceHolder>

        <!-- Grilla de Vehículos Comprados -->
        <asp:PlaceHolder ID="phGarageLista" runat="server" Visible="true">
            <div class="grid-autos-comprados">
                <asp:Repeater ID="repAutosComprados" runat="server" OnItemCommand="repAutosComprados_ItemCommand">
                    <ItemTemplate>
                        <article class="auto-comprado-card">
                            <div class="card-imagen-wrapper">
                                <span class="badge-marca-flotante"><%# Eval("Marca") %></span>
                                <span class="badge-propietario"><i class="fa-solid fa-circle-check"></i> <asp:Label ID="lblBadgeEnPropiedad" runat="server" Text="En Propiedad" /></span>
                                <img src='<%# ObtenerImagenAuto(Eval("AuBase")) %>' alt='<%# Eval("Modelo") %>' />
                            </div>

                            <div class="card-cuerpo">
                                <div class="card-titulo-seccion">
                                    <h2><%# Eval("Modelo") %> (<%# Eval("Anio") %>)</h2>
                                </div>

                                <div class="card-specs-fila">
                                    <div class="spec-item">
                                        <i class="fa-solid fa-gauge-high"></i>
                                        <span><%# Eval("VelocidadMaxima") %> km/h</span>
                                    </div>
                                    <div class="spec-item">
                                        <i class="fa-solid fa-bolt"></i>
                                        <span><%# Eval("Potencia") %> HP</span>
                                    </div>
                                    <div class="spec-item">
                                        <i class="fa-solid fa-stopwatch"></i>
                                        <span><%# Eval("Aceleracion0a100") %>s</span>
                                    </div>
                                </div>

                                <div class="card-mejoras-seccion">
                                    <h4><i class="fa-solid fa-sliders"></i> <asp:Label ID="lblMejorasInstaladas" runat="server" Text="Mejoras Instaladas:" /></h4>
                                    <div class="lista-tags-mejoras">
                                        <%# FormatearTagsMejoras(Eval("NombreExtra")) %>
                                    </div>
                                </div>

                                <div class="card-datos-factura">
                                    <div class="factura-row">
                                        <span><asp:Label ID="lblFacturaNum" runat="server" Text="Factura N°:" /></span>
                                        <strong><%# Eval("NumeroFactura") %></strong>
                                    </div>
                                    <div class="factura-row">
                                        <span><asp:Label ID="lblVentaNum" runat="server" Text="Venta N°:" /></span>
                                        <strong><%# Eval("NumeroVenta") %></strong>
                                    </div>
                                    <div class="factura-row">
                                        <span><asp:Label ID="lblFechaCompra" runat="server" Text="Fecha:" /></span>
                                        <span><%# Eval("FechaCompra", "{0:dd/MM/yyyy HH:mm}") %></span>
                                    </div>
                                    <div class="factura-row">
                                        <span><asp:Label ID="lblMetodoPagoCard" runat="server" Text="Método de Pago:" /></span>
                                        <span><%# Eval("MetodoPago") %></span>
                                    </div>
                                    <div class="factura-total-row">
                                        <span><asp:Label ID="lblMontoAbonado" runat="server" Text="Monto Abonado:" /></span>
                                        <span>$<%# Eval("TotalPagado", "{0:N0}") %></span>
                                    </div>
                                </div>

                                <!-- Acciones: Descarga Instantánea de Factura PDF y Vista de Factura -->
                                <div class="card-acciones">
                                    <a href='<%# "DescargarFactura.ashx?num=" + Eval("NumeroFactura") + "&vta=" + Eval("NumeroVenta") %>' 
                                       class="btn-descargar-pdf">
                                        <i class="fa-solid fa-file-pdf"></i> <asp:Label ID="lblBtnDescargarPdf" runat="server" Text="Descargar Factura (PDF)" />
                                    </a>

                                    <a href='<%# "DescargarFactura.ashx?num=" + Eval("NumeroFactura") + "&vta=" + Eval("NumeroVenta") + "&modo=ver" %>' 
                                       target="_blank" class="btn-ver-factura">
                                        <i class="fa-solid fa-file-invoice"></i> <asp:Label ID="lblBtnVerFactura" runat="server" Text="Ver Factura" />
                                    </a>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:PlaceHolder>
    </main>
</asp:Content>