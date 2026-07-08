<%@ Page Title="BackupRestore" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BackupRestore.aspx.cs" Inherits="RedLine.Web.BackupRestore" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="Content/BackupRestore.css" rel="stylesheet" type="text/css" />

    <h1>Gestion de Backup y Restore</h1>
    <div>
            <h2>Copia de Seguridad</h2>
            <asp:TextBox ID="txtRutaBackup" runat="server" Width="350px"></asp:TextBox>
            <asp:Button ID="btnGenerar" runat="server" Text="Generar copia" OnClick="btnGenerar_Click" />

            <hr />

            <h2>Restaurar Base de Datos</h2>
            <asp:FileUpload ID="fileUploadRestore" runat="server" />
            <asp:Button ID="btnRestaurar" runat="server" Text="Restaurar" OnClick="btnRestaurar_Click" />

            <hr />
            <asp:Label ID="lblEstado" runat="server" Font-Bold="true"></asp:Label>
        </div>
</asp:Content>