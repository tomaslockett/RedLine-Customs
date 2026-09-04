<%@ Page Title="LogIn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="RedLine.Web.LogIn" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/LogIn.css" rel="stylesheet" type="text/css" />

    <main class="login-contenedor">
        <section class="login-caja">
            <h1 class="login-titulo"><asp:Label ID="lblTitulo" runat="server" Text="Iniciar sesión" /></h1>

            <div class="login-campo">
                <label><asp:Label ID="lblEmail" runat="server" Text="Email" /></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="login-input" />
            </div>

            <div class="login-campo">
                <label><asp:Label ID="lblPassword" runat="server" Text="Contraseña" /></label>
                <div class="login-pass-wrapper">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="login-input" />
                    <i class="fa-solid fa-eye-slash login-toggle-icon" id="togglePass"></i>
                </div>
            </div>

            <div style="text-align: center; margin-bottom: 15px; margin-top: 15px;">
                <asp:Label ID="lblMensaje" runat="server" ForeColor="#D93416" Font-Size="0.9em" Text=""></asp:Label>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="login-boton" OnClick="BtnLogin_Click" />

            <div style="text-align: center; margin-top: 25px;">
                <p style="color: darkgray; font-size: 0.9rem; margin: 0;">
                    <asp:Label ID="lblNoTienesCuenta" runat="server" Text="¿No tienes cuenta?" />
                    <asp:HyperLink ID="linkRegistro" runat="server" NavigateUrl="RegistroCliente.aspx" Style="color: #D93416; text-decoration: none; font-weight: bold;" Text="Regístrate aquí" />
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
