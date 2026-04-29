<%@ Page Title= "LogIn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="RedLine.Web.LogIn"  %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main class="loginContainer">
        <section class="loginBox">
            <h1 class="loginTitulo">Iniciar sesión</h1>

            <div class="campo">
                <label>Email o Usuario</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="inputLogin" />
            </div>

           <div class="campo" style="position: relative;">
                <label>Contraseña</label>
                 <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="inputLogin" />
                    <i class="fa-solid fa-eye-slash" id="togglePass" style="position: absolute; right: 10px; top: 35px; cursor: pointer; color: rgba(255,255,255,0.3);"></i>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" CssClass="btnLogin" OnClick="BtnLogin_Click" />

            <div style="text-align: center; margin-top: 20px;">
                <p style="color: darkgray; font-size: 0.9rem;">
                    ¿No tienes cuenta? <a href="RegistroUsuario.aspx" style="color: #D93416; text-decoration: none; font-weight: bold;">Regístrate aquí</a>
                </p>
            </div>
        </section>
    </main>

    <script>
        document.getElementById('togglePass').addEventListener('click', function () {
            const passInput = document.getElementById('<%= txtPassword.ClientID %>');
            const type = passInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passInput.setAttribute('type', type);
            
            if (type === 'text') {
                this.style.color = "white";
                this.classList.replace('fa-eye-slash', 'fa-eye');
            } else {
                this.style.color = "rgba(255,255,255,0.3)";
                this.classList.replace('fa-eye', 'fa-eye-slash');
            }
        });
    </script>
</asp:Content>
