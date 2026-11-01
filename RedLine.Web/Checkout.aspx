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

            <!-- Conversor de divisas de referencia comercial -->
            <div class="currency-converter-box">
                <div class="converter-header">
                    <i class="fa-solid fa-coins icon-gold"></i>
                    <span class="converter-title"><asp:Label ID="lblTituloConversor" runat="server" Text="Moneda de referencia" /></span>
                </div>
                <div class="converter-body">
                    <asp:DropDownList ID="ddlMonedaReferencia" runat="server" ClientIDMode="Static" CssClass="form-control-sm">
                        <asp:ListItem Value="USD" Selected="True" Text="USD - Dólar Estadounidense" />
                        <asp:ListItem Value="ARS_MEP" Text="ARS - Dólar MEP ($1.220)" />
                        <asp:ListItem Value="ARS_OFICIAL" Text="ARS - Dólar Oficial ($1.050)" />
                        <asp:ListItem Value="EUR" Text="EUR - Euro (€0.92)" />
                        <asp:ListItem Value="BRL" Text="BRL - Real Brasileño (R$5.75)" />
                    </asp:DropDownList>
                    <div class="converter-details">
                        <div class="converter-row">
                            <span class="c-label"><asp:Label ID="lblEquivalenteReferencial" runat="server" Text="Importe referencial:" /></span>
                            <span id="lblResultadoConversion" class="c-val">-</span>
                        </div>
                        <div class="converter-row">
                            <span class="c-label"><asp:Label ID="lblTipoCambioAplicado" runat="server" Text="Tipo de cambio aplicado:" /></span>
                            <span id="lblDetalleTipoCambio" class="c-val">-</span>
                        </div>
                    </div>
                    <p class="converter-legal-note">
                        <i class="fa-solid fa-circle-info"></i>
                        <asp:Label ID="lblAvisoMonedaOficial" runat="server" Text="Facturación y cobro oficial emitidos en USD" />
                    </p>
                </div>
            </div>
        </section>

        <section class="payment-methods-card">
            <h2><asp:Label ID="lblMetodosPago" runat="server" Text="Métodos de Pago" /></h2>
            
            <!-- HiddenFields para total USD y contrato de método de pago -->
            <asp:HiddenField ID="hfTotalUsd" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="hfMetodoPago" runat="server" ClientIDMode="Static" Value="Tarjeta de Crédito" />
            <asp:HiddenField ID="hfPlanFinanciacionSeleccionado" runat="server" ClientIDMode="Static" />

            <!-- Pestaña 1: Tarjeta de Crédito / Débito -->
            <div class="payment-option active" data-target="credit-card-panel">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-credit-card icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionTarjeta" runat="server" Text="Tarjeta de Crédito / Débito" /></span>
                </div>

                <div id="credit-card-panel" class="option-panel open">
                    <div class="form-group">
                        <label><asp:Label ID="lblNombreEnTarjeta" runat="server" Text="Nombre en la tarjeta" /></label>
                        <asp:TextBox ID="txtCardName" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="Juan Pérez"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label><asp:Label ID="lblNumeroTarjeta" runat="server" Text="Número de tarjeta" /></label>
                        <div class="input-with-brands">
                            <asp:TextBox ID="txtCardNumber" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="1234 5678 9012 3456" maxlength="19"></asp:TextBox>
                            <div class="card-brands">
                                <i id="brandVisa" class="fa-brands fa-cc-visa brand-visa"></i>
                                <i id="brandMastercard" class="fa-brands fa-cc-mastercard brand-mastercard"></i>
                                <i id="brandAmex" class="fa-brands fa-cc-amex brand-amex"></i>
                            </div>
                        </div>
                        <div id="cardValidationStatus" class="card-validation-status"></div>
                    </div>
                    <div class="form-row-grid">
                        <div class="form-group">
                            <label><asp:Label ID="lblExpiracion" runat="server" Text="Expiración (MM/AA)" /></label>
                            <asp:TextBox ID="txtExpiry" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="12/28" maxlength="5"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label><asp:Label ID="lblCvv" runat="server" Text="CVV" /></label>
                            <asp:TextBox ID="txtCvv" runat="server" ClientIDMode="Static" CssClass="form-control" placeholder="123" maxlength="4"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Pestaña 2: Transferencia Bancaria -->
            <div class="payment-option" data-target="bank-panel">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-landmark icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionTransferencia" runat="server" Text="Transferencia Bancaria" /></span>
                </div>
                <div id="bank-panel" class="option-panel">
                    <div class="bank-details-card">
                        <div class="bank-grid">
                            <div class="bank-item"><i class="fa-solid fa-building"></i> <asp:Label ID="lblTitularBanco" runat="server" Text="Titular: REDLINE CUSTOMS S.A." /></div>
                            <div class="bank-item"><i class="fa-solid fa-id-card"></i> <asp:Label ID="lblCuitBanco" runat="server" Text="CUIT: 30-71829384-9" /></div>
                            <div class="bank-item"><i class="fa-solid fa-university"></i> <asp:Label ID="lblEntidadBanco" runat="server" Text="Entidad: Banco Santander Río" /></div>
                            <div class="bank-item"><i class="fa-solid fa-money-check"></i> <asp:Label ID="lblCbuBanco" runat="server" Text="CBU: 0720123400000012345678" /></div>
                            <div class="bank-item"><i class="fa-solid fa-tag"></i> <asp:Label ID="lblAliasBanco" runat="server" Text="Alias: REDLINE.CUSTOMS.OFICIAL" /></div>
                        </div>

                        <div id="bankTransferEquivalentBox" class="bank-equivalent-box">
                            <span class="eq-label"><asp:Label ID="lblMontoSugeridoPesos" runat="server" Text="Monto sugerido a transferir (ARS):" /></span>
                            <span id="lblMontoSugeridoValor" class="eq-val">-</span>
                        </div>

                        <p class="panel-placeholder-text">
                            <i class="fa-solid fa-circle-check text-green"></i>
                            <asp:Label ID="lblInstruccionTransferencia" runat="server" Text="Realizá la transferencia y envianos el comprobante para habilitar la entrega." />
                        </p>
                    </div>
                </div>
            </div>

            <!-- Pestaña 3: Financiación -->
            <div class="payment-option" data-target="financing-panel">
                <div class="option-header">
                    <span class="radio-indicator"></span>
                    <i class="fa-solid fa-percent icon-left"></i>
                    <span class="option-title"><asp:Label ID="lblOpcionFinanciacion" runat="server" Text="Financiación" /></span>
                </div>
                <div id="financing-panel" class="option-panel">
                    <div class="financing-simulator-box">
                        <div class="sim-header">
                            <h4><asp:Label ID="lblTituloSimulador" runat="server" Text="Simulador de Financiación Prendaria" /></h4>
                            <p class="simulator-subtitle"><asp:Label ID="lblSubtituloSimulador" runat="server" Text="Seleccioná el plazo y anticipo para simular cuotas fijas en USD." /></p>
                        </div>

                        <div class="form-row-grid">
                            <div class="form-group">
                                <label><asp:Label ID="lblCuotasPlazo" runat="server" Text="Plazo:" /></label>
                                <asp:DropDownList ID="ddlCuotasFinanciacion" runat="server" ClientIDMode="Static" CssClass="form-control">
                                    <asp:ListItem Value="12" Text="12 cuotas (TNA 9.5%)" />
                                    <asp:ListItem Value="24" Selected="True" Text="24 cuotas (TNA 11.5%)" />
                                    <asp:ListItem Value="36" Text="36 cuotas (TNA 13.5%)" />
                                    <asp:ListItem Value="48" Text="48 cuotas (TNA 15.5%)" />
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label><asp:Label ID="lblAnticipoInicial" runat="server" Text="Anticipo (%):" /></label>
                                <asp:DropDownList ID="ddlAnticipoFinanciacion" runat="server" ClientIDMode="Static" CssClass="form-control">
                                    <asp:ListItem Value="20" Text="20%" />
                                    <asp:ListItem Value="30" Selected="True" Text="30%" />
                                    <asp:ListItem Value="40" Text="40%" />
                                    <asp:ListItem Value="50" Text="50%" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="financing-results-grid">
                            <div class="result-card">
                                <span class="res-title"><asp:Label ID="lblAnticipoCalculado" runat="server" Text="Anticipo inicial (USD):" /></span>
                                <span id="lblFinAnticipoVal" class="res-val">$0</span>
                            </div>
                            <div class="result-card">
                                <span class="res-title"><asp:Label ID="lblCapitalFinanciar" runat="server" Text="Saldo financiado (USD):" /></span>
                                <span id="lblFinCapitalVal" class="res-val">$0</span>
                            </div>
                            <div class="result-card highlight-card">
                                <span class="res-title"><asp:Label ID="lblValorCuotaMensual" runat="server" Text="Cuota mensual estimada (USD):" /></span>
                                <span id="lblFinCuotaVal" class="res-val text-red">$0</span>
                            </div>
                            <div class="result-card">
                                <span class="res-title"><asp:Label ID="lblTasaTnaCft" runat="server" Text="TNA / CFT estimado:" /></span>
                                <span id="lblFinTasasVal" class="res-val">11.5% / 13.5%</span>
                            </div>
                            <div class="result-card full-span">
                                <span class="res-title"><asp:Label ID="lblTotalPlanEstimado" runat="server" Text="Total estimado del plan (USD):" /></span>
                                <span id="lblFinTotalPlanVal" class="res-val">$0</span>
                            </div>
                        </div>

                        <p class="financing-legal-note">
                            <i class="fa-solid fa-file-contract"></i>
                            <asp:Label ID="lblAvisoFiscalFinanciacion" runat="server" Text="La factura oficial del vehículo se emite por el valor de lista + IVA en USD. El saldo se instrumenta mediante mutuo prendario." />
                        </p>
                    </div>
                </div>
            </div>

            <asp:Label ID="lblErrorCheckout" runat="server" Visible="false" Style="display: block; color: #ef4444; margin-top: 12px; font-weight: 600;" />
            <asp:Button ID="btnProcesarPago" runat="server" CssClass="btnConfirmar" Text="Confirmar y Pagar Total" OnClick="btnProcesarPago_Click" Style="width: 100%; margin-top: 20px; display: block;" />

            <footer class="security-badges">
                <span class="badge badge-ssl"><i class="fa-solid fa-lock"></i> <asp:Label ID="lblBadgeSsl" runat="server" Text="SSL Seguro" /></span>
                <span class="badge badge-3d"><i class="fa-solid fa-shield-halved"></i> <asp:Label ID="lblBadge3d" runat="server" Text="3D Secure" /></span>
            </footer>
        </section>
    </main>

    <script src="Scripts/Checkout.js"></script>
</asp:Content>
