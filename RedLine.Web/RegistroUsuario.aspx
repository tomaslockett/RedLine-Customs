<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistroUsuario.aspx.cs" Inherits="RedLine.Web.RegistroUsuario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <div class="loginContainer" style="height: auto; padding: 40px 0;"> 
        <section class="loginBox" style="width: 650px;">
            <h1 class="loginTitulo">Registrarse</h1>
            
            <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 20px;">
                <div class="campo">
                    <label>DNI</label>
                    <asp:TextBox ID="txtDNI" runat="server" CssClass="inputLogin" placeholder="Sin puntos"></asp:TextBox>
                </div>

                <div class="campo">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="inputLogin" placeholder="ejemplo@mail.com"></asp:TextBox>
                </div>

                <div class="campo">
                    <label>Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="inputLogin"></asp:TextBox>
                </div>

                <div class="campo">
                    <label>Apellido</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="inputLogin"></asp:TextBox>
                </div>

                <div class="campo">
                    <label>Teléfono</label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="inputLogin"></asp:TextBox>
                </div>

                <div class="campo">
                    <label>Dirección</label>
                    <asp:TextBox ID="txtDireccion" runat="server" CssClass="inputLogin"></asp:TextBox>
                </div>

                <div class="campo">
                    <label>Contraseña</label>
                    <div style="position: relative;">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="inputLogin" TextMode="Password" placeholder="••••••••" style="width: 100%;"></asp:TextBox>
                        <i class="fa-solid fa-eye-slash" id="togglePass" style="position: absolute; right: 10px; top: 12px; cursor: pointer; color: rgba(255,255,255,0.3);"></i>
                    </div>
                </div>

                <div class="campo">
                    <label>Confirmar Contraseña</label>
                    <div style="position: relative;">
                        <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="inputLogin" TextMode="Password" placeholder="••••••••" style="width: 100%;"></asp:TextBox>
                        <i class="fa-solid fa-eye-slash" id="togglePassConfirm" style="position: absolute; right: 10px; top: 12px; cursor: pointer; color: rgba(255,255,255,0.3);"></i>
                    </div>
                </div>
            </div>

            <div style="text-align: center; margin-bottom: 10px;">
               <asp:Label ID="lblMensaje" runat="server" ForeColor="#D93416" Font-Size="0.9em" Text=""></asp:Label>
            </div>

            <div style="margin-top: 10px; display: flex; justify-content: center;">
               <asp:Button ID="btnRegistrar" runat="server" Text="Crear Cuenta" CssClass="btnLogin" OnClick="btnRegistrar_Click" />
            </div>

            <div style="text-align:center; margin-top:15px;">
                <a href="LogIn.aspx" style="color: darkgray; text-decoration: none; font-size: 0.9rem;">
                    ¿Ya tienes cuenta? Inicia sesión
                </a>
            </div>
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
                if (type === 'text') {
                    this.style.color = "white";
                    this.classList.replace('fa-eye-slash', 'fa-eye');
                } else {
                    this.style.color = "rgba(255,255,255,0.3)";
                    this.classList.replace('fa-eye', 'fa-eye-slash');
                }
            });
        }
        setupToggle('togglePass', '<%= txtPassword.ClientID %>');
        setupToggle('togglePassConfirm', '<%= txtPasswordConfirm.ClientID %>');
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>