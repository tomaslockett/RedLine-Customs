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
            
            <h1><asp:Label ID="lblTituloPagoExitoso" runat="server" Text="¡Pago Exitoso!" /></h1>
            <p class="success-subtitle"><asp:Label ID="lblSubtituloPagoExitoso" runat="server" Text="Tu compra se ha procesado correctamente" /></p>

            <div class="transaction-box">
                <span class="transaction-label"><asp:Label ID="lblEtiquetaTransaccion" runat="server" Text="Número de transacción" /></span>
                <asp:Label ID="lblNumeroTransaccion" runat="server" CssClass="transaction-id" Text="TXN-1780260457772-YQRUBNLIX" />
            </div>

            <div class="success-actions">
                <asp:Button ID="btnHome" runat="server" CssClass="btn-success-primary" Text="Volver al Inicio" OnClick="btnHome_Click" />
                <asp:Button ID="btnGarage" runat="server" CssClass="btn-success-secondary" Text="Ir a mi Garage Virtual" OnClick="btnGarage_Click" />
            </div>

            <p class="success-footer-text"><asp:Label ID="lblEmailConfirmacion" runat="server" Text="Recibirás un email de confirmación en breve" /></p>
        </section>
    </main>
</asp:Content>