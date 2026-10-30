<%@ Page Title="Pago Exitoso" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PagoExitoso.aspx.cs" Inherits="RedLine.Web.PagoExitoso" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link rel="stylesheet" href="Content/Checkout.css" />

    <main class="success-screen-wrapper">
        <section class="success-card">
            <div class="success-icon-container">
                <div class="success-icon-circle">
                    <i class="fa-solid fa-check"></i>
                </div>
            </div>
            
            <h1><asp:Label ID="lblTituloPagoExitoso" runat="server" Text="¡Compra Confirmada con Éxito!" /></h1>
            <p class="success-subtitle"><asp:Label ID="lblSubtituloPagoExitoso" runat="server" Text="Tu vehículo ha sido asignado a tu cuenta y la factura ha sido emitida." /></p>

            <div class="transaction-box">
                <span class="transaction-label"><asp:Label ID="lblEtiquetaTransaccion" runat="server" Text="Número de Venta / Transacción" /></span>
                <asp:Label ID="lblNumeroTransaccion" runat="server" CssClass="transaction-id" Text="TXN-2026-REDLINE" />
            </div>

            <div class="success-actions">
                <asp:Button ID="btnGarage" runat="server" CssClass="btn-success-primary" Text="🚗 Ver Mis Autos Comprados" OnClick="btnGarage_Click" />
                <asp:Button ID="btnVerFactura" runat="server" CssClass="btn-success-secondary" Text="📄 Ver Factura en Pantalla" OnClick="btnVerFactura_Click" />
                <asp:Button ID="btnDescargarPdf" runat="server" CssClass="btn-success-primary" Text="📥 Descargar Factura (PDF)" OnClick="btnDescargarPdf_Click" Style="background:#1e293b;" />
                <asp:Button ID="btnHome" runat="server" CssClass="btn-secundario-outline" Text="Volver al Catálogo" OnClick="btnHome_Click" />
            </div>

            <p class="success-footer-text"><asp:Label ID="lblEmailConfirmacion" runat="server" Text="El vehículo ya está disponible en tu sección exclusiva de autos comprados." /></p>
        </section>
    </main>
</asp:Content>
