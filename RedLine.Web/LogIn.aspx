<%@ Page Title="LogIn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="RedLine.Web.LogIn" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="loginContainer">
        <section class="loginBox">
            <h1 class="loginTitulo">Iniciar sesión</h1>

            <div class="campo">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="inputLogin" />
            </div>

            <div class="campo">
                <label>Contraseña</label>
                <div style="position: relative; width: 100%;">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="inputLogin" style="width: 100%; box-sizing: border-box;" />
                    <i class="fa-solid fa-eye-slash" id="togglePass" style="position: absolute; right: 10px; top: 10px; cursor: pointer; color: rgba(255,255,255,0.3);"></i>
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btnLogin" OnClick="BtnLogin_Click" />

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
