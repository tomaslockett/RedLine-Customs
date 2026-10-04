<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistroCliente.aspx.cs" Inherits="RedLine.Web.RegistroCliente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    <link href="/Content/RegistroCliente.css" rel="stylesheet" type="text/css" runat="server" />

    <div class="registro-contenedor"> 
        <section class="registro-caja">
            <h1 class="registro-titulo">
                <asp:Label ID="lblTituloRegistro" runat="server" Text="Registrarse" />
            </h1>
            
            <div class="registro-grid">
                <div class="registro-campo">
                    <label><asp:Label ID="lblDniRegistro" runat="server" Text="DNI" /></label>
                    <asp:TextBox ID="txtDNI" runat="server" CssClass="registro-input" placeholder="Sin puntos"></asp:TextBox>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblEmailRegistro" runat="server" Text="Email" /></label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="registro-input" placeholder="ejemplo@mail.com"></asp:TextBox>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblNombreRegistro" runat="server" Text="Nombre" /></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="registro-input"></asp:TextBox>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblApellidoRegistro" runat="server" Text="Apellido" /></label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="registro-input"></asp:TextBox>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblTelefonoRegistro" runat="server" Text="Teléfono" /></label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="registro-input"></asp:TextBox>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblDireccionRegistro" runat="server" Text="Dirección" /></label>
                    <asp:TextBox ID="txtDireccion" runat="server" CssClass="registro-input"></asp:TextBox>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblPasswordRegistro" runat="server" Text="Contraseña" /></label>
                    <div class="registro-pass-wrapper">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="registro-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        <i class="fa-solid fa-eye-slash registro-toggle-icon" id="togglePass"></i>
                    </div>
                </div>

                <div class="registro-campo">
                    <label><asp:Label ID="lblPasswordConfirmRegistro" runat="server" Text="Confirmar Contraseña" /></label>
                    <div class="registro-pass-wrapper">
                        <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="registro-input" TextMode="Password" placeholder="••••••••"></asp:TextBox>
                        <i class="fa-solid fa-eye-slash registro-toggle-icon" id="togglePassConfirm"></i>
                    </div>
                </div>
            </div>

            <div class="registro-mensaje">
               <asp:Label ID="lblMensaje" runat="server" ForeColor="#D93416" Font-Size="0.9em" Text=""></asp:Label>
            </div>

            <div class="registro-boton-contenedor">
               <asp:Button ID="btnRegistrar" runat="server" Text="Crear Cuenta" CssClass="registro-boton" OnClick="btnRegistrar_Click" />
            </div>

            <div class="registro-link-contenedor">
                <asp:HyperLink ID="lnkIrLogin" runat="server" NavigateUrl="LogIn.aspx" CssClass="registro-link" Text="¿Ya tienes cuenta? Inicia sesión" />
            </div>
        </section>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="<%= ResolveUrl("~/Scripts/RegistroCliente.js") %>"></script>

    <script>
        setupTogglePassword('togglePass', '<%= txtPassword.ClientID %>');
        setupTogglePassword('togglePassConfirm', '<%= txtPasswordConfirm.ClientID %>');
    </script>
</asp:Content>