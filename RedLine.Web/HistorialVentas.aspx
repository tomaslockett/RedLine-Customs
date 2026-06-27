<%@ Page Title="Historial de Ventas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistorialVentas.aspx.cs" Inherits="RedLine.Web.HistorialVentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   <link href="Content/HistorialVentas.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="encabezado-pagina">
            <h1 class="titulo">Historial de Ventas</h1>
            <p class="subTitulo">Registro inmutable de transacciones y auditoría de configuraciones.</p>
        </div>

       <div class="kpi-contenedor">
                <div class="kpi-tarjeta">
                    <span class="kpi-etiqueta">Ventas Totales</span>
                    <asp:Label ID="lblVentasTotales" runat="server" CssClass="kpi-valor">0</asp:Label>
           </div>
                <div class="kpi-tarjeta destacados">
                    <span class="kpi-etiqueta">Ingresos Totales</span>
                    <asp:Label ID="lblIngresosTotales" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
            </div>
                <div class="kpi-tarjeta">
                    <span class="kpi-etiqueta">Ticket Promedio</span>
                    <asp:Label ID="lblTicketPromedio" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
            </div>
                <div class="kpi-tarjeta">
                    <span class="kpi-etiqueta">Ventas Este Mes</span>
                    <asp:Label ID="lblVentasEsteMes" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
        </div>

        <div class="tarjeta-chasis filtros-panel">
            <div class="filtros-grid">
                <div class="filtro-grupo busqueda-principal">
                    <label>Buscar venta</label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="input-taller" placeholder="N° Venta, cliente o vehículo..."></asp:TextBox>
                </div>
                <div class="filtro-grupo">
                    <label>Fecha Desde</label>
                    <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="input-taller" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filtro-grupo">
                    <label>Fecha Hasta</label>
                    <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="input-taller" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filtro-grupo acciones-filtro">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-primario" />
                </div>
            </div>
        </div>

        <div class="tarjeta-chasis panel-tabla">
            <div class="tabla-contenedor">
                <asp:GridView ID="dgvVentas" runat="server" AutoGenerateColumns="False" CssClass="gridview-custom">
                    <Columns>
                        <asp:BoundField DataField="NumeroVenta" HeaderText="N° Venta" ItemStyle-Width="12%" HeaderStyle-CssClass="col-header" />
                        
                        <asp:TemplateField HeaderText="Cliente" ItemStyle-Width="23%">
                            <ItemTemplate>
                                <%# Eval("Cliente.Nombre") %> <%# Eval("Cliente.Apellido") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Vehículo Base" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <%# Eval("AutoBase.Nombre") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Personalización" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <span class="badge-custom"><%# Eval("AutoPersonalizado.NombreExtra") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="IVA" HeaderText="IVA" DataFormatString="{0}%" ItemStyle-Width="6%" />
                        
                        <asp:BoundField DataField="Total" HeaderText="Total" DataFormatString="US$ {0:N2}" ItemStyle-Width="12%" ItemStyle-CssClass="col-total" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
