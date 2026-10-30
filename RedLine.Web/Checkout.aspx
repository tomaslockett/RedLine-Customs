<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="RedLine.Web.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link rel="stylesheet" href="Content/Checkout.css" />

    <header class="checkout-header">
        <h1><asp:Label ID="lblTituloCheckout" runat="server" Text="Checkout" /></h1>
        <p><asp:Label ID="lblSubtituloCheckout" runat="server" Text="Completa tu compra de forma segura y transparente" /></p>
    </header>

    <!-- Contenedor de Advertencias / Alertas dinámicas -->
    <div id="divAlertaCheckout" runat="server" class="checkout-alert" style="display:none; max-width: 1200px; margin: 0 auto 25px auto;">
        <asp:Literal ID="litMensajeAlerta" runat="server"></asp:Literal>
    </div>

    <div id="jsAlertaCheckout" class="checkout-alert" style="display:none; max-width: 1200px; margin: 0 auto 25px auto;">
        <strong><i class="fa-solid fa-triangle-exclamation"></i> Por favor verifique los siguientes datos:</strong>
        <ul id="jsListaErrores"></ul>
    </div>

    <asp:HiddenField ID="hfMetodoPagoSeleccionado" runat="server" Value="Tarjeta" />

    <main class="checkout-container">
        <!-- Tarjeta de Resumen de la Orden -->
        <section class="order-summary-card">
            <h2><asp:Label ID="lblResumenOrden" runat="server" Text="Resumen de Orden" /></h2>
            <div class="product-gallery">
                <img id="imgAuto" runat="server" alt="Auto" class="product-img" />
            </div>
            <div class="product-info">
                <h3 id="lblModelo" runat="server">Modelo</h3>
                <p class="subtitle"><asp:Label ID="lblConfigPersonalizada" runat="server" Text="Configuración personalizada con adicionales" /></p>
            </div>
            <div class="price-breakdown">
                <div class="price-row">
                    <span><asp:Label ID="lblTextoPrecioBase" runat="server" Text="Precio Base Vehículo" /></span>
                    <span id="lblPrecioBase" runat="server">$0</span>
                </div>

                <div id="contenedorExtras" runat="server"></div>

                <hr class="divider" />
                <div class="price-row text-bold">
                    <span><asp:Label ID="lblTextoSubtotal" runat="server" Text="Subtotal Neto" /></span>
                    <span id="lblSubtotal" runat="server">$0</span>
                </div>
                <div class="price-row">
                    <span><asp:Label ID="lblTextoIva" runat="server" Text="IVA (21%)" /></span>
                    <span id="lblIva" runat="server">$0</span>
                </div>
                <hr class="divider" />
                <div class="price-row total-row">
                    <span><asp:Label ID="lblTextoTotal" runat="server" Text="Total a Pagar" /></span>
                    <span id="lblTotal" runat="server" class="text-red">$0</span>
                </div>
            </div>
        </section>

        <!-- Tarjeta de Métodos de Pago -->
        <section class="payment-methods-card">
            <h2><asp:Label ID="lblMetodosPago" runat="server" Text="Métodos de Pago Disponibles" /></h2>

            <!-- Opción 1: Tarjeta de Crédito / Débito -->
            <div class="payment-option active" data-target="credit-card-panel" data-metodo="Tarjeta">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-credit-card icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionTarjeta" runat="server" Text="Tarjeta de Crédito / Débito" /></span>
                </div>

                <div id="credit-card-panel" class="option-panel open">
                    <div class="form-group">
                        <label><asp:Label ID="lblNombreEnTarjeta" runat="server" Text="Nombre y Apellido del Titular (como figura en la tarjeta) *" /></label>
                        <asp:TextBox ID="txtCardName" runat="server" CssClass="form-control" placeholder="Ej: JUAN PEREZ" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><asp:Label ID="lblDniTitular" runat="server" Text="DNI del Titular *" /></label>
                        <asp:TextBox ID="txtCardDni" runat="server" CssClass="form-control" placeholder="Ej: 38456123" maxlength="8" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label><asp:Label ID="lblNumeroTarjeta" runat="server" Text="Número de Tarjeta (16 dígitos) *" /></label>
                        <div class="input-with-brands">
                            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control" placeholder="1234 5678 9012 3456" maxlength="19" ClientIDMode="Static"></asp:TextBox>
                            <div class="card-brands">
                                <i class="fa-brands fa-cc-visa brand-visa"></i>
                                <i class="fa-brands fa-cc-mastercard brand-mastercard"></i>
                            </div>
                        </div>
                    </div>

                    <div class="form-row-grid">
                        <div class="form-group">
                            <label><asp:Label ID="lblExpiracion" runat="server" Text="Vencimiento (MM/AA) *" /></label>
                            <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control" placeholder="12/28" maxlength="5" ClientIDMode="Static"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label><asp:Label ID="lblCvv" runat="server" Text="CVV (Código de seguridad) *" /></label>
                            <asp:TextBox ID="txtCvv" runat="server" CssClass="form-control" placeholder="123" maxlength="4" ClientIDMode="Static"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label><asp:Label ID="lblCuotas" runat="server" Text="Plan de Pago" /></label>
                        <asp:DropDownList ID="ddlCuotas" runat="server" CssClass="form-control" ClientIDMode="Static">
                            <asp:ListItem Value="1" Text="1 Pago (Sin recargo)" />
                            <asp:ListItem Value="3" Text="3 Cuotas fijas" />
                            <asp:ListItem Value="6" Text="6 Cuotas fijas" />
                            <asp:ListItem Value="12" Text="12 Cuotas preferenciales" />
                        </asp:DropDownList>
                    </div>

                    <asp:Button ID="btnProcesarPago" runat="server" CssClass="btn-success-primary" Text="Confirmar y Pagar con Tarjeta" OnClientClick="return validarFormularioTarjeta();" OnClick="btnProcesarPago_Click" Style="margin-top: 22px;" />
                </div>
            </div>

            <!-- Opción 2: Transferencia Bancaria -->
            <div class="payment-option" data-target="bank-panel" data-metodo="Transferencia">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-landmark icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionTransferencia" runat="server" Text="Transferencia Bancaria Inmediata" /></span>
                </div>

                <div id="bank-panel" class="option-panel">
                    <div class="bank-info-box">
                        <h4><i class="fa-solid fa-building-columns"></i> Datos de la Cuenta Oficial RedLine</h4>
                        <div class="bank-info-grid">
                            <div class="bank-info-item">
                                <span>Banco</span>
                                <span>Banco Santander Río</span>
                            </div>
                            <div class="bank-info-item">
                                <span>Titular</span>
                                <span>RedLine Customs S.A.</span>
                            </div>
                            <div class="bank-info-item">
                                <span>CUIT</span>
                                <span>30-71829384-9</span>
                            </div>
                            <div class="bank-info-item">
                                <span>Alias CBU</span>
                                <span>REDLINE.CUSTOMS.OFICIAL</span>
                            </div>
                            <div class="bank-info-item" style="grid-column: span 2;">
                                <span>CBU</span>
                                <span>0720123920000001234567</span>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label><asp:Label ID="lblTransferTitular" runat="server" Text="Nombre y Apellido del Titular Emisor *" /></label>
                        <asp:TextBox ID="txtTransferTitular" runat="server" CssClass="form-control" placeholder="Ej: Maria Lopez" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="form-row-grid">
                        <div class="form-group">
                            <label><asp:Label ID="lblTransferDni" runat="server" Text="DNI / CUIT Emisor *" /></label>
                            <asp:TextBox ID="txtTransferDni" runat="server" CssClass="form-control" placeholder="Ej: 27381234564" maxlength="11" ClientIDMode="Static"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label><asp:Label ID="lblTransferBanco" runat="server" Text="Banco o Billetera de Origen *" /></label>
                            <asp:TextBox ID="txtTransferBanco" runat="server" CssClass="form-control" placeholder="Ej: Santander, Galicia, MP" ClientIDMode="Static"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <label><asp:Label ID="lblTransferCbu" runat="server" Text="CBU o CVU de la Cuenta de Origen (22 dígitos) *" /></label>
                        <asp:TextBox ID="txtTransferCbu" runat="server" CssClass="form-control" placeholder="0000000000000000000000" maxlength="22" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="form-row-grid">
                        <div class="form-group">
                            <label><asp:Label ID="lblTransferComprobante" runat="server" Text="N° de Comprobante / Transacción *" /></label>
                            <asp:TextBox ID="txtTransferComprobante" runat="server" CssClass="form-control" placeholder="Ej: TRF-9823412" ClientIDMode="Static"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label><asp:Label ID="lblTransferTelefono" runat="server" Text="Teléfono de Contacto / WhatsApp *" /></label>
                            <asp:TextBox ID="txtTransferTelefono" runat="server" CssClass="form-control" placeholder="Ej: 1154789012" maxlength="15" ClientIDMode="Static"></asp:TextBox>
                        </div>
                    </div>

                    <asp:Button ID="btnProcesarTransferencia" runat="server" CssClass="btn-success-primary" Text="Confirmar y Registrar Transferencia" OnClientClick="return validarFormularioTransferencia();" OnClick="btnProcesarTransferencia_Click" Style="margin-top: 22px;" />
                </div>
            </div>

            <!-- Opción 3: Financiación (Deshabilitada/Próximamente según requerimiento) -->
            <div class="payment-option" data-target="financing-panel" style="opacity: 0.65; cursor: not-allowed;">
                <div class="option-header" style="cursor: not-allowed;">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-percent icon-left"></i>
                    <span class="option-title">
                        <asp:Label ID="lblOpcionFinanciacion" runat="server" Text="Financiación a Medida" />
                        <span class="badge-proximamente">Próximamente</span>
                    </span>
                </div>
                <div id="financing-panel" class="option-panel">
                    <p class="panel-placeholder-text">
                        <asp:Label ID="lblInfoFinanciacion" runat="server" Text="Nuestro módulo de financiación personalizada y aprobación crediticia en línea está en proceso de integración. Por favor, utilice Tarjeta de Crédito/Débito o Transferencia Bancaria para concretar su compra." />
                    </p>
                </div>
            </div>

            <footer class="security-badges">
                <span class="badge badge-ssl"><i class="fa-solid fa-lock"></i> <asp:Label ID="lblBadgeSsl" runat="server" Text="Conexión Encriptada SSL" /></span>
                <span class="badge badge-3d"><i class="fa-solid fa-shield-halved"></i> <asp:Label ID="lblBadge3d" runat="server" Text="Protección de Compra Garantizada" /></span>
            </footer>
        </section>
    </main>

    <script src="Scripts/Checkout.js?v=2.0"></script>
</asp:Content>
