<%@ Page Title="Gestión de Eventos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionEventos.aspx.cs" Inherits="RedLine.Web.GestionEventos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/GestionEventos.css" rel="stylesheet" type="text/css" runat="server" />

    <div class="contenedor-eventos-main">
        <section class="panel-bitacora-recuadro">
            <h1 class="titulo-bitacora">Bitácora de Eventos</h1>

            <asp:UpdatePanel ID="upBitacora" runat="server">
                <ContentTemplate>
                    <div class="grid-filtros">
                        <div class="campo-bitacora">
                            <label>Usuario (Email)</label>
                            <asp:TextBox ID="txtFiltroUsuario" runat="server" CssClass="control-input-eventos" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo-bitacora">
                            <label>Fecha (dd/mm/aaaa)</label>
                            <asp:TextBox ID="txtFiltroFecha" runat="server" CssClass="control-input-eventos" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo-bitacora">
                            <label>Módulo</label>
                            <asp:TextBox ID="txtFiltroModulo" runat="server" CssClass="control-input-eventos" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo-bitacora">
                            <label>Actividad</label>
                            <asp:TextBox ID="txtFiltroActividad" runat="server" CssClass="control-input-eventos" AutoPostBack="true" OnTextChanged="Filtro_Changed"></asp:TextBox>
                        </div>
                        <div class="campo-bitacora">
                            <label>Criticidad</label>
                            <asp:DropDownList ID="ddlFiltroCri" runat="server" CssClass="control-input-eventos" AutoPostBack="true" OnSelectedIndexChanged="Filtro_Changed">
                                <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1 - Baja" Value="1"></asp:ListItem>
                                <asp:ListItem Text="2 - Media" Value="2"></asp:ListItem>
                                <asp:ListItem Text="3 - Alta" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="botonera-bitacora">
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar Filtros" CssClass="boton-bitacora-accion boton-bitacora-secundario" OnClick="btnLimpiar_Click" />
                    </div>

                    <div class="contenedor-tabla-scroll">
                        <asp:GridView ID="gvEventos" runat="server" AutoGenerateColumns="False" CssClass="tabla-eventos-custom" GridLines="None">
                            <Columns>
                                <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                                <asp:BoundField DataField="Modulo" HeaderText="Módulo" />
                                <asp:BoundField DataField="Actividad" HeaderText="Actividad" />
                                <asp:TemplateField HeaderText="Criticidad">
                                    <ItemTemplate>
                                        <span class="etiqueta-criticidad" style="<%# GetCriColor(Eval("Criticidad")) %>">
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
</asp:Content>