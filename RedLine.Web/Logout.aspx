<%@ Page Title="Logout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Logout.aspx.cs" Inherits="RedLine.Web.Logout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="Content/Logout.css" rel="stylesheet" />

    <div class="logout-container">
        <div class="logout-card">
            <h1>Cerrar Sesión</h1>
            <p>¿Está seguro de que desea cerrar su sesión?</p>
            <div class="logout-actions">
                <asp:Button ID="btnConfirmar" runat="server" Text="Sí, salir" OnClick="btnConfirmar_Click" CssClass="btn-danger" />
                <asp:Button ID="btnCancelar" runat="server" Text="No, volver" OnClick="btnCancelar_Click" CssClass="btn-secondary" />
            </div>
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        </div>
    </div>
</asp:Content>
