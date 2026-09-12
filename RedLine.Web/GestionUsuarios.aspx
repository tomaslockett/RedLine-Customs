<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="RedLine.Web.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/GestionUsuarios.css" rel="stylesheet" type="text/css" runat="server" />

    <div class="bloque-administracion-usuarios">
        <section class="panel-usuarios-recuadro">
            <h1 class="titulo-gestion-usuarios">
                <asp:Label ID="lblTituloGestionUsuarios" runat="server" Text="Administración de Personal" />
            </h1>

            <div class="grid-formulario">
                <div class="casilla-formulario-usuario">
                    <label><asp:Label ID="lblDniUsuario" runat="server" Text="DNI" /></label>
                    <asp:TextBox ID="txtDNI" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label><asp:Label ID="lblNombreUsuarioForm" runat="server" Text="Nombre" /></label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label><asp:Label ID="lblApellidoUsuarioForm" runat="server" Text="Apellido" /></label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label><asp:Label ID="lblEmailUsuarioForm" runat="server" Text="Email" /></label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label><asp:Label ID="lblRolUsuarioForm" runat="server" Text="Rol" /></label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="control-entrada-usuario">
                    </asp:DropDownList>
                </div>
            </div>

            <div class="botonera-contenedor" style="display: flex; gap: 10px; justify-content: center; margin-bottom: 25px;">
                <asp:Button ID="btnAgregar" runat="server" Text="Guardar Usuario" CssClass="boton-control-usuario" OnClick="btnAgregar_Click" style="background: #D93416;" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Cancelar / Limpiar" CssClass="boton-control-usuario" OnClick="btnLimpiar_Click" style="background: #444;" />
            </div>

            <asp:Label ID="lblMensaje" runat="server" style="display: block; margin-bottom: 15px; font-weight: bold; color: #D93416;"></asp:Label>

            <div class="contenedor-tabla-scroll">
                <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="grilla-usuarios-oscura" DataKeyNames="ID" OnRowDeleting="gvUsuarios_RowDeleting" OnSelectedIndexChanged="gvUsuarios_SelectedIndexChanged" OnRowCommand="gvUsuarios_RowCommand" OnRowDataBound="gvUsuarios_RowDataBound" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="col_UsuarioID" HeaderStyle-CssClass="columna-oculta" ItemStyle-CssClass="columna-oculta" />
                        <asp:BoundField DataField="DNI" HeaderText="col_UsuarioDNI" />
                        <asp:BoundField DataField="Nombre" HeaderText="col_UsuarioNombre" />
                        <asp:BoundField DataField="Apellido" HeaderText="col_UsuarioApellido" />
                        <asp:BoundField DataField="Email" HeaderText="col_UsuarioEmail" />
                        <asp:TemplateField HeaderText="col_UsuarioPerfil">
                            <ItemTemplate>
                                <asp:Label ID="lblNombrePerfil" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="col_UsuarioEstado">
                            <ItemTemplate>
                                <span style='<%# (bool)Eval("Activo") ? "color: #28a745;" : "color: #dc3545;" %>'>
                                    <%# (bool)Eval("Activo") ? Traducir("estado_usuario_activo") : Traducir("estado_usuario_inactivo") %>
                                </span>
                                <br />
                                <span style='<%# (bool)Eval("Bloqueado") ? "color: #ffc107;" : "color: #aaa;" %>'>
                                    <%# (bool)Eval("Bloqueado") ? Traducir("estado_usuario_bloqueado") : Traducir("estado_usuario_desbloqueado") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="col_UsuarioAcciones">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Select" Text='<%# Traducir("btn_grid_editar") %>' CssClass="link-accion-fila" />
                                <asp:LinkButton ID="btnBorrarUsuario" runat="server" CommandName="Delete" Text='<%# Traducir("btn_grid_borrar") %>' CssClass="link-accion-fila link-accion-eliminar" />
                                <div style="margin-top: 5px;">
                                    <asp:LinkButton runat="server" CommandName="CambiarEstado" CommandArgument='<%# Eval("ID") %>' Text='<%# (bool)Eval("Activo") ? Traducir("btn_grid_desactivar") : Traducir("btn_grid_activar") %>' CssClass="link-accion-fila" />
                                    <asp:LinkButton runat="server" CommandName="Desbloquear" CommandArgument='<%# Eval("ID") %>' Text='<%# Traducir("btn_grid_desbloquear") %>' CssClass="link-accion-fila" style="color: #ffc107;" Visible='<%# Eval("Bloqueado") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </section>
    </div>
</asp:Content>