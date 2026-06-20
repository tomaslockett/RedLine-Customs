<%@ Page Title="Cambiar Contraseña" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CambioContraseña.aspx.cs" Inherits="RedLine.Web.CambioContraseña" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/CambioContraseña.css" rel="stylesheet" type="text/css" runat="server" />

    <div class="loginContainer">
        <section class="loginBox">
            <h1 class="loginTitulo">Cambiar Contraseña</h1>

            <div class="campo">
                <label>Contraseña Actual</label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtPassActual" runat="server" TextMode="Password" CssClass="inputLogin" />
                    <i class="fa-solid fa-eye-slash toggle-icon" id="toggleActual"></i>
                </div>
            </div>

            <div class="campo">
                <label>Nueva Contraseña</label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtPassNueva" runat="server" TextMode="Password" CssClass="inputLogin" />
                    <i class="fa-solid fa-eye-slash toggle-icon" id="toggleNueva"></i>
                </div>
            </div>

            <div class="campo">
                <label>Confirmar Nueva Contraseña</label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtPassConfirm" runat="server" TextMode="Password" CssClass="inputLogin" />
                    <i class="fa-solid fa-eye-slash toggle-icon" id="toggleConfirm"></i>
                </div>
            </div>

            <div style="text-align: center; margin-bottom: 15px;">
                <asp:Label ID="lblMensaje" runat="server" ForeColor="#D93416" Font-Size="0.9em" Text=""></asp:Label>
            </div>

            <asp:Button ID="btnCambiar" runat="server" Text="Actualizar Contraseña" CssClass="btnLogin" OnClick="btnCambiar_Click" />
        </section>
    </div>

    <script>
        function setupToggle(iconId, inputId) {
            const icon = document.getElementById(iconId);
            const input = document.getElementById(inputId);
            if (!icon || !input) return;
            icon.addEventListener('click', function () {
                const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
                input.setAttribute('type', type);
                this.classList.toggle('fa-eye-slash');
                this.classList.toggle('fa-eye');
                this.style.color = type === 'text' ? 'white' : 'rgba(255,255,255,0.3)';
            });
        }
        setupToggle('toggleActual', '<%= txtPassActual.ClientID %>');
        setupToggle('toggleNueva', '<%= txtPassNueva.ClientID %>');
        setupToggle('toggleConfirm', '<%= txtPassConfirm.ClientID %>');
    </script>
</asp:Content>
