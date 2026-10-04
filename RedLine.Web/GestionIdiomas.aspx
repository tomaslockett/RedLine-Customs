<%@ Page Title="Gestión de Idiomas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionIdiomas.aspx.cs" Inherits="RedLine.Web.GestionIdiomas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/GestionIdiomas.css" rel="stylesheet" type="text/css" />

    <main class="gestion-idiomas-container">
        <h1><asp:Label ID="lblTituloPagina" runat="server" Text="Gestión de Idiomas y Traducciones" /></h1>

        <div style="margin-bottom: 15px;">
            <asp:Label ID="lblEstado" runat="server" Font-Bold="true"></asp:Label>
        </div>

        <section class="seccion-card">
            <h2><asp:Label ID="lblSubtituloNuevoIdioma" runat="server" Text="Crear Nuevo Idioma" /></h2>
            <div class="form-inline">
                <asp:TextBox ID="txtNombreIdioma" runat="server" CssClass="input-text" Placeholder="Nombre del idioma (ej: Portugués)" Width="300px" />
                <asp:Button ID="btnCrearIdioma" runat="server" Text="Guardar Idioma" CssClass="btn-rojo" OnClick="BtnCrearIdioma_Click" />
            </div>
        </section>

        <section class="seccion-card">
            <h2><asp:Label ID="lblSubtituloTraducciones" runat="server" Text="Editar Traducciones" /></h2>
            
            <div class="form-inline" style="margin-bottom: 20px; display: flex; flex-wrap: wrap; gap: 15px; align-items: center;">
                <div>
                    <label><asp:Label ID="lblSeleccionarIdioma" runat="server" Text="Idioma a traducir:" /></label>
                    <asp:DropDownList ID="ddlIdiomaDestino" runat="server" CssClass="input-text" AutoPostBack="true" OnSelectedIndexChanged="DdlIdiomaDestino_SelectedIndexChanged" />
                </div>

                <div>
                    <label><asp:Label ID="lblFiltroPagina" runat="server" Text="Filtrar por Pantalla:" /></label>
                    <asp:DropDownList ID="ddlFiltroPagina" runat="server" CssClass="input-text" AutoPostBack="true" OnSelectedIndexChanged="DdlFiltroPagina_SelectedIndexChanged">
                      <asp:ListItem Value="" Text="-- Todas las pantallas --" />
                            <asp:ListItem Value="Global" Text="Global / Alertas del Sistema" />
                            <asp:ListItem Value="Site.Master" Text="Menú Master / Navegación" />
                            <asp:ListItem Value="Login.aspx" Text="Login" />
                            <asp:ListItem Value="RegistroClientes.aspx" Text="Registro de Clientes" />
                            <asp:ListItem Value="CambioContraseña.aspx" Text="Cambio de Contraseña" />
                            <asp:ListItem Value="Catalogo.aspx" Text="Catálogo" />
                            <asp:ListItem Value="PersonalizarAuto.aspx" Text="Personalizar Auto" />
                            <asp:ListItem Value="CrearAuto.aspx" Text="Crear Auto" />
                            <asp:ListItem Value="Checkout.aspx" Text="Checkout / Compra" />
                            <asp:ListItem Value="PagoExitoso.aspx" Text="Confirmación de Pago" />
                            <asp:ListItem Value="HistorialVentas.aspx" Text="Historial de Ventas" />
                            <asp:ListItem Value="Inventario.aspx" Text="Gestión de Inventario" />
                            <asp:ListItem Value="GestionClientes.aspx" Text="Gestión de Clientes" />
                            <asp:ListItem Value="GestionUsuarios.aspx" Text="Gestión de Usuarios" />
                            <asp:ListItem Value="GestionPerfiles.aspx" Text="Gestión de Perfiles / Familias" />
                            <asp:ListItem Value="GestionIdiomas.aspx" Text="Gestión de Idiomas" />
                            <asp:ListItem Value="GestionEventos.aspx" Text="Bitácora de Eventos" />
                            <asp:ListItem Value="BackupRestore.aspx" Text="Backup & Restore" />
                            <asp:ListItem Value="RecuperarDV.aspx" Text="Dígito Verificador / Integridad" />
                    </asp:DropDownList>
                </div>

                <div>
                    <label><asp:Label ID="lblBuscar" runat="server" Text="Buscar clave:" /></label>
                    <asp:TextBox ID="txtFiltro" runat="server" CssClass="input-text" AutoPostBack="true" OnTextChanged="TxtFiltro_TextChanged" Placeholder="Filtrar por clave o descripción..." />
                </div>

                <div>
                    <asp:CheckBox ID="chkSoloSinTraducir" runat="server" Text=" Solo sin traducir" AutoPostBack="true" OnCheckedChanged="ChkSoloSinTraducir_CheckedChanged" Style="color: #ffffff;" />
                </div>
            </div>

            <asp:GridView ID="gvTraducciones" runat="server" AutoGenerateColumns="false" CssClass="grid-traducciones" DataKeyNames="IdEtiqueta">
                <Columns>
                    <asp:BoundField DataField="Etiqueta.Pagina" HeaderText="Pantalla / Módulo" ReadOnly="true" ItemStyle-Width="15%" />
                    <asp:BoundField DataField="Etiqueta.Clave" HeaderText="Clave de Etiqueta" ReadOnly="true" ItemStyle-Width="20%" />
                    <asp:BoundField DataField="Etiqueta.Descripcion" HeaderText="Descripción / Uso" ReadOnly="true" ItemStyle-Width="25%" />
                    
                    <asp:TemplateField HeaderText="Tipo" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <span class='<%# Convert.ToBoolean(Eval("Etiqueta.EsMensajeAlerta")) ? "badge-alerta" : "badge-control" %>'>
                                <%# Convert.ToBoolean(Eval("Etiqueta.EsMensajeAlerta")) ? "Alerta / Mensaje" : "Control Visual" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Traducción" ItemStyle-Width="30%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtTextoTraducido" runat="server" Text='<%# Bind("Texto") %>' CssClass="input-text" Width="95%" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div style="margin-top: 20px; text-align: right;">
                <asp:Button ID="btnGuardarTraducciones" runat="server" Text="Guardar Todas las Traducciones" CssClass="btn-rojo" OnClick="BtnGuardarTraducciones_Click" />
            </div>
        </section>
    </main>
</asp:Content>