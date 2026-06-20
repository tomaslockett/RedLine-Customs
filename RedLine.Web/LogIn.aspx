<%@ Page Title="LogIn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="RedLine.Web.LogIn" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/LogIn.css" rel="stylesheet" type="text/css" />

    <main class="login-contenedor">
        <section class="login-caja">
            <h1 class="login-titulo">Iniciar sesión</h1>

            <div class="login-campo">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="login-input" />
            </div>

            <div class="login-campo">
                <label>Contraseña</label>
                <div class="login-pass-wrapper">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="login-input" />
                    <i class="fa-solid fa-eye-slash login-toggle-icon" id="togglePass"></i>
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="login-boton" OnClick="BtnLogin_Click" />

            <div style="text-align: center; margin-top: 25px;">
                <p style="color: darkgray; font-size: 0.9rem; margin: 0;">
                    ¿No tienes cuenta? <a href="RegistroCliente.aspx" style="color: #D93416; text-decoration: none; font-weight: bold;">Regístrate aquí</a>
                </p>
            </div>
        </section>
    </main>

    <script>
        document.getElementById('togglePass').addEventListener('click', function () {
            const passInput = document.getElementById('<%= txtPassword.ClientID %>');
            const icon = this;
            const type = passInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passInput.setAttribute('type', type);

            if (type === 'text') {
                icon.style.color = "white";
                icon.classList.replace('fa-eye-slash', 'fa-eye');
            } else {
                icon.style.color = "rgba(255,255,255,0.3)";
                icon.classList.replace('fa-eye', 'fa-eye-slash');
            }
        });
    </script>
</asp:Content>
