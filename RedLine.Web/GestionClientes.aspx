<%@ Page Title="Gestion De Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="RedLine.Web.GestionClientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/GestionClientes.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="encabezado-pagina">
            <h1 class="titulo">Consulta de Clientes</h1>
            <p class="subTitulo">Administración del registro de clientes activos y auditoría de cuentas.</p>
        </div>

        <div class="kpi-contenedor">
            <div class="kpi-tarjeta destacados">
                <span class="kpi-etiqueta">Total Clientes</span>
                <asp:Label ID="lblTotalClientes" runat="server" CssClass="kpi-valor">12</asp:Label>
            </div>
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta">Activos Este Mes</span>
                <asp:Label ID="lblActivosMes" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta">Nuevos Este Mes</span>
                <asp:Label ID="lblNuevosMes" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
        </div>

        <div class="tarjeta-chasis panel-tabla">
            <div class="tabla-contenedor">
                <asp:GridView ID="dgvClientes" runat="server" AutoGenerateColumns="False" CssClass="gridview-custom" OnRowCommand="dgvClientes_RowCommand" OnSelectedIndexChanged="dgvClientes_SelectedIndexChanged">
                    <Columns>

                        <asp:TemplateField HeaderText="ID" ItemStyle-Width="10%">
                            <ItemTemplate>
                                <span class="badge-id">USR-<%# (Container.DataItemIndex + 1).ToString("D4") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="DNI" HeaderText="DNI" ItemStyle-Width="10%" />
                        <asp:BoundField DataField="Nombre" HeaderText="Nombre" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="Apellido" HeaderText="Apellido" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="Email" HeaderText="Email" ItemStyle-Width="20%" />
                        <asp:BoundField DataField="Telefono" HeaderText="Teléfono" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="Direccion" HeaderText="Dirección" ItemStyle-Width="18%" />
                        
                        <asp:TemplateField HeaderText="Contraseña" ItemStyle-Width="8%">
                            <ItemTemplate>
                                <span class="contrasena-oculta">••••••••</span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="8%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnVer" runat="server" CssClass="btn-link-ver" CommandName="VerPerfil" CommandArgument='<%# Eval("DNI") %>'>Ver Perfil</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            
            <div class="tabla-footer-resumen">
                <asp:Label ID="lblResumenPaginacion" runat="server" Text="Mostrando 1–12 de 12 clientes"></asp:Label>
            </div>
        </div>

    </div>
</asp:Content>
