<%@ Page Title="BackupRestore" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BackupRestore.aspx.cs" Inherits="RedLine.Web.BackupRestore" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="Content/BackupRestore.css" rel="stylesheet" type="text/css" />

    <h1><asp:Label ID="lblTitulo" runat="server" Text="Gestión de Backup y Restore" /></h1>
    <div>
        <h2><asp:Label ID="lblSubtituloBackup" runat="server" Text="Copia de Seguridad" /></h2>
        <asp:TextBox ID="txtRutaBackup" runat="server" Width="350px"></asp:TextBox>
        <asp:Button ID="btnGenerar" runat="server" Text="Generar copia" OnClick="btnGenerar_Click" />

        <hr />

        <h2><asp:Label ID="lblSubtituloRestore" runat="server" Text="Restaurar Base de Datos" /></h2>
        
        <div style="display: inline-flex; align-items: center; gap: 10px;">
            <asp:FileUpload ID="fileUploadRestore" runat="server" Style="display: none;" onchange="actualizarNombreArchivo(this)" />
            <label for="<%= fileUploadRestore.ClientID %>" class="login-boton" style="cursor: pointer; padding: 8px 15px; margin: 0; display: inline-block;">
                <asp:Label ID="lblBotonSeleccionar" runat="server" Text="Seleccionar archivo" />
            </label>
            <span id="lblNombreArchivo" style="color: white; font-size: 0.9rem;">
                <asp:Label ID="lblSinArchivo" runat="server" Text="Sin archivos seleccionados" />
            </span>
        </div>

        <asp:Button ID="btnRestaurar" runat="server" Text="Restaurar" OnClick="btnRestaurar_Click" />

        <hr />
        <asp:Label ID="lblEstado" runat="server" Font-Bold="true"></asp:Label>
    </div>

    <script>
        function actualizarNombreArchivo(input) {
            var lbl = document.getElementById('lblNombreArchivo');
            if (input.files && input.files.length > 0) {
                lbl.innerText = input.files[0].name;
            }
        }
    </script>
</asp:Content>