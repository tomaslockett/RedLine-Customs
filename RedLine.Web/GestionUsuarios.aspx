<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="RedLine.Web.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="loginContainer" style="height: auto; padding: 30px;">
        <section class="loginBox" style="width: 95%; max-width: 1200px;">
            <h1 class="loginTitulo">Administración de Personal</h1>

            <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px; margin-bottom: 20px; text-align: left;">
                <div class="campo">
                    <label>DNI</label>
                    <asp:TextBox ID="txtDNI" runat="server" CssClass="inputLogin"></asp:TextBox>
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
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="inputLogin"></asp:TextBox>
                </div>
                <div class="campo">
                    <label>Rol</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="inputLogin" style="background: #1A1F26; height: 45px;">
                        <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                        <asp:ListItem Text="Empleado" Value="Empleado"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div style="margin-bottom: 25px; display: flex; gap: 10px; justify-content: center;">
                <asp:Button ID="btnAgregar" runat="server" Text="Guardar Usuario" CssClass="btnLogin" OnClick="btnAgregar_Click" style="width: 180px;" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Cancelar / Limpiar" CssClass="btnLogin" OnClick="btnLimpiar_Click" style="width: 180px; background: #444;" />
            </div>

            <asp:Label ID="lblMensaje" runat="server" style="display: block; margin-bottom: 15px; font-weight: bold;"></asp:Label>

            <div style="overflow-x: auto; background: #111; border-radius: 8px; padding: 10px;">
                <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="tablaDark" 
                    DataKeyNames="ID" OnRowDeleting="gvUsuarios_RowDeleting" OnSelectedIndexChanged="gvUsuarios_SelectedIndexChanged" 
                    OnRowCommand="gvUsuarios_RowCommand" Width="100%" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" HeaderStyle-CssClass="hide" ItemStyle-CssClass="hide" />
                        <asp:BoundField DataField="DNI" HeaderText="DNI" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Rol" HeaderText="Rol" />
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <span style='<%# (bool)Eval("Activo") ? "color: #28a745;" : "color: #dc3545;" %>'>
                                    <%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %>
                                </span>
                                <br />
                                <span style='<%# (bool)Eval("Bloqueado") ? "color: #ffc107;" : "color: #aaa;" %>'>
                                    <%# (bool)Eval("Bloqueado") ? "BLOQUEADO" : "Desbloqueado" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="Select" Text="Editar" CssClass="btnAccion" />
                                <asp:LinkButton runat="server" CommandName="Delete" Text="Borrar" CssClass="btnAccion btnEliminar" OnClientClick="return confirm('¿Eliminar usuario?');" />
                                <div style="margin-top: 5px;">
                                    <asp:LinkButton runat="server" CommandName="CambiarEstado" CommandArgument='<%# Eval("ID") %>' Text='<%# (bool)Eval("Activo") ? "Desactivar" : "Activar" %>' CssClass="btnAccion" style="color: #007bff;" />
                                    <asp:LinkButton runat="server" CommandName="Desbloquear" CommandArgument='<%# Eval("ID") %>' Text="Desbloquear" CssClass="btnAccion" style="color: #ffc107;" Visible='<%# Eval("Bloqueado") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </section>
    </div>

    <style>
        .tablaDark { background: transparent; color: #ccc; border-collapse: collapse; font-size: 0.9em; }
        .tablaDark th { background: #D93416; color: white; padding: 12px; text-transform: uppercase; letter-spacing: 1px; }
        .tablaDark td { padding: 12px; border-bottom: 1px solid #222; }
        .btnAccion { font-weight: bold; text-decoration: none; margin-right: 8px; font-size: 0.85em; cursor: pointer; display: inline-block; }
        .btnEliminar { color: #dc3545; }
        .hide { display: none; }
    </style>
</asp:Content>