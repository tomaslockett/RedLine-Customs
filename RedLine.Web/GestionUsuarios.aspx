<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="RedLine.Web.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <link href="/Content/GestionUsuarios.css" rel="stylesheet" type="text/css" runat="server" />

    <div class="bloque-administracion-usuarios">
        <section class="panel-usuarios-recuadro">
            <h1 class="titulo-gestion-usuarios">Administración de Personal</h1>

            <div class="grid-formulario">
                <div class="casilla-formulario-usuario">
                    <label>DNI</label>
                    <asp:TextBox ID="txtDNI" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label>Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label>Apellido</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="control-entrada-usuario"></asp:TextBox>
                </div>
                <div class="casilla-formulario-usuario">
                    <label>Rol</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="control-entrada-usuario">
                        <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                        <asp:ListItem Text="Empleado" Value="Empleado"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="botonera-contenedor" style="display: flex; gap: 10px; justify-content: center; margin-bottom: 25px;">
                <asp:Button ID="btnAgregar" runat="server" Text="Guardar Usuario" CssClass="boton-control-usuario" OnClick="btnAgregar_Click" style="background: #D93416;" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Cancelar / Limpiar" CssClass="boton-control-usuario" OnClick="btnLimpiar_Click" style="background: #444;" />
            </div>

            <asp:Label ID="lblMensaje" runat="server" style="display: block; margin-bottom: 15px; font-weight: bold; color: #D93416;"></asp:Label>

            <div class="contenedor-tabla-scroll">
                <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="grilla-usuarios-oscura" 
                    DataKeyNames="ID" OnRowDeleting="gvUsuarios_RowDeleting" OnSelectedIndexChanged="gvUsuarios_SelectedIndexChanged" 
                    OnRowCommand="gvUsuarios_RowCommand" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" HeaderStyle-CssClass="columna-oculta" ItemStyle-CssClass="columna-oculta" />
                        <asp:BoundField DataField="DNI" HeaderText="DNI" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:TemplateField HeaderText="Perfil">
                            <ItemTemplate>
                                <%# Eval("Perfil") != null ? Eval("Perfil.Nombre") : "Sin perfil" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <span style='<%# (bool)Eval("Activo") ? "color: #28a745;" : "color: #dc3545;" %>'><%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %></span>
                                <br />
                                <span style='<%# (bool)Eval("Bloqueado") ? "color: #ffc107;" : "color: #aaa;" %>'><%# (bool)Eval("Bloqueado") ? "BLOQUEADO" : "Desbloqueado" %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Select" Text="Editar" CssClass="link-accion-fila" />
                                <asp:LinkButton runat="server" CommandName="Delete" Text="Borrar" CssClass="link-accion-fila link-accion-eliminar" OnClientClick="return confirm('¿Eliminar usuario?');" />
                                <div style="margin-top: 5px;">
                                    <asp:LinkButton runat="server" CommandName="CambiarEstado" CommandArgument='<%# Eval("ID") %>' Text='<%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>' CssClass="link-accion-fila" />
                                    <asp:LinkButton runat="server" CommandName="Desbloquear" CommandArgument='<%# Eval("ID") %>' Text="Desbloquear" CssClass="link-accion-fila" style="color: #ffc107;" Visible='<%# Eval("Bloqueado") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </section>
    </div>
</asp:Content>