<%@ Page Title="Gestión de Eventos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionEventos.aspx.cs" Inherits="RedLine.Web.GestionEventos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="loginContainer" style="height: auto; padding: 30px;">
        <section class="loginBox" style="width: 95%; max-width: 1300px;">
            <h1 class="loginTitulo">Bitácora de Eventos</h1>

            <asp:UpdatePanel ID="upBitacora" runat="server">
                <ContentTemplate>
                    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 10px; margin-bottom: 20px; text-align: left;">
                        <div class="campo">
                            <label>Usuario (Email)</label>
                            <asp:TextBox ID="txtFiltroUsuario" runat="server" CssClass="inputLogin" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo">
                            <label>Fecha (dd/mm/aaaa)</label>
                            <asp:TextBox ID="txtFiltroFecha" runat="server" CssClass="inputLogin" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo">
                            <label>Módulo</label>
                            <asp:TextBox ID="txtFiltroModulo" runat="server" CssClass="inputLogin" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo">
                            <label>Actividad</label>
                            <asp:TextBox ID="txtFiltroActividad" runat="server" CssClass="inputLogin" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo">
                            <label>Criticidad</label>
                            <asp:DropDownList ID="ddlFiltroCri" runat="server" CssClass="inputLogin" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed" style="background: #1A1F26; height: 45px;">
                                <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1 - Baja" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2 - Media" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3 - Alta" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar Filtros" CssClass="btnLogin" OnClick="btnLimpiar_Click" style="width: 180px; background: #444;" />
                    </div>

                    <div style="overflow-x: auto; background: #111; border-radius: 8px; padding: 10px;">
                        <asp:GridView ID="gvEventos" runat="server" AutoGenerateColumns="False" CssClass="tablaDark" Width="100%" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                                <asp:BoundField DataField="Modulo" HeaderText="Módulo" />
                                <asp:BoundField DataField="Actividad" HeaderText="Actividad" />
                                <asp:TemplateField HeaderText="Criticidad">
                                    <ItemTemplate>
                                        <span style="<%# GetCriColor(Eval("Criticidad")) %>">
                                            <%# Eval("Criticidad") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </section>
    </div>

    <style>
        .tablaDark { background: transparent; color: #ccc; border-collapse: collapse; font-size: 0.85em; }
        .tablaDark th { background: #D93416; color: white; padding: 12px; text-transform: uppercase; }
        .tablaDark td { padding: 12px; border-bottom: 1px solid #222; }
    </style>
</asp:Content>