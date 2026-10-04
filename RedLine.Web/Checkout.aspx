<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="RedLine.Web.Checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link rel="stylesheet" href="Content/Checkout.css" />

    <header class="checkout-header">
        <h1><asp:Label ID="lblTituloCheckout" runat="server" Text="Checkout" /></h1>
        <p><asp:Label ID="lblSubtituloCheckout" runat="server" Text="Completa tu compra de forma segura" /></p>
    </header>

    <main class="checkout-container">
        <section class="order-summary-card">
            <h2><asp:Label ID="lblResumenOrden" runat="server" Text="Resumen de Orden" /></h2>
            <div class="product-gallery">
                <img id="imgAuto" runat="server" alt="Auto" class="product-img" />
            </div>
            <div class="product-info">
                <h3 id="lblModelo" runat="server">Modelo</h3>
                <p class="subtitle"><asp:Label ID="lblConfigPersonalizada" runat="server" Text="Configuración personalizada" /></p>
            </div>
            <div class="price-breakdown">
                <div class="price-row">
                    <span><asp:Label ID="lblTextoPrecioBase" runat="server" Text="Precio Base" /></span>
                    <span id="lblPrecioBase" runat="server">$0</span>
                </div>

                <div id="contenedorExtras" runat="server"></div>

                <hr class="divider" />
                <div class="price-row text-bold">
                    <span><asp:Label ID="lblTextoSubtotal" runat="server" Text="Subtotal" /></span>
                    <span id="lblSubtotal" runat="server">$0</span>
                </div>
                <div class="price-row">
                    <span><asp:Label ID="lblTextoIva" runat="server" Text="IVA (21%)" /></span>
                    <span id="lblIva" runat="server">$0</span>
                </div>
                <hr class="divider" />
                <div class="price-row total-row">
                    <span><asp:Label ID="lblTextoTotal" runat="server" Text="Total" /></span>
                    <span id="lblTotal" runat="server" class="text-red">$0</span>
                </div>
            </div>
        </section>

        <section class="payment-methods-card">
            <h2><asp:Label ID="lblMetodosPago" runat="server" Text="Métodos de Pago" /></h2>

            <div class="payment-option active" data-target="credit-card-panel">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-credit-card icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionTarjeta" runat="server" Text="Tarjeta de Crédito / Débito" /></span>
                </div>

                <div id="credit-card-panel" class="option-panel open">
                    <div class="form-group">
                        <label><asp:Label ID="lblNombreEnTarjeta" runat="server" Text="Nombre en la tarjeta" /></label>
                        <asp:TextBox ID="txtCardName" runat="server" CssClass="form-control" placeholder="Juan Pérez"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label><asp:Label ID="lblNumeroTarjeta" runat="server" Text="Número de tarjeta" /></label>
                        <div class="input-with-brands">
                            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control" placeholder="1234 5678 9012 3456" maxlength="19"></asp:TextBox>
                            <div class="card-brands">
                                <i class="fa-brands fa-cc-visa brand-visa"></i>
                                <i class="fa-brands fa-cc-mastercard brand-mastercard"></i>
                            </div>
                        </div>
                    </div>
                    <div class="form-row-grid">
                        <div class="form-group">
                            <label><asp:Label ID="lblExpiracion" runat="server" Text="Expiración (MM/AA)" /></label>
                            <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control" placeholder="12/28" maxlength="5"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label><asp:Label ID="lblCvv" runat="server" Text="CVV" /></label>
                            <asp:TextBox ID="txtCvv" runat="server" CssClass="form-control" placeholder="123" maxlength="4"></asp:TextBox>
                        </div>
                    </div>
                    <asp:Button ID="btnProcesarPago" runat="server" CssClass="btnConfirmar" Text="Confirmar y Pagar Total" OnClick="btnProcesarPago_Click" Style="width: 100%; margin-top: 20px; display: block;" />
                </div>
            </div>

            <div class="payment-option" data-target="bank-panel">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-landmark icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionTransferencia" runat="server" Text="Transferencia Bancaria" /></span>
                </div>
                <div id="bank-panel" class="option-panel">
                    <p class="panel-placeholder-text"><asp:Label ID="lblInfoTransferencia" runat="server" Text="Información para la transferencia bancaria..." /></p>
                </div>
            </div>

            <div class="payment-option" data-target="financing-panel">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-percent icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionFinanciacion" runat="server" Text="Financiación" /></span>
                </div>
                <div id="financing-panel" class="option-panel">
                    <p class="panel-placeholder-text"><asp:Label ID="lblInfoFinanciacion" runat="server" Text="Opciones de financiación disponibles..." /></p>
                </div>
            </div>

            <footer class="security-badges">
                <span class="badge badge-ssl"><i class="fa-solid fa-lock"></i> <asp:Label ID="lblBadgeSsl" runat="server" Text="SSL Seguro" /></span>
                <span class="badge badge-3d"><i class="fa-solid fa-shield-halved"></i> <asp:Label ID="lblBadge3d" runat="server" Text="3D Secure" /></span>
            </footer>
        </section>
    </main>

    <script src="Scripts/Checkout.js"></script>
</asp:Content>