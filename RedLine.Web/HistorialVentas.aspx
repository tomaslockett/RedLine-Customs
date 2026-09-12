<%@ Page Title="Historial de Ventas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistorialVentas.aspx.cs" Inherits="RedLine.Web.HistorialVentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/HistorialVentas.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="encabezado-pagina">
            <h1 class="titulo"><asp:Label ID="lblTituloHistorialVentas" runat="server" Text="Historial de Ventas" /></h1>
            <p class="subTitulo"><asp:Label ID="lblSubtituloHistorialVentas" runat="server" Text="Registro inmutable de transacciones y auditoría de configuraciones." /></p>
        </div>

        <div class="kpi-contenedor">
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiVentasTotales" runat="server" Text="Ventas Totales" /></span>
                <asp:Label ID="lblVentasTotales" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
            <div class="kpi-tarjeta destacados">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiIngresosTotales" runat="server" Text="Ingresos Totales" /></span>
                <asp:Label ID="lblIngresosTotales" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
            </div>
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiTicketPromedio" runat="server" Text="Ticket Promedio" /></span>
                <asp:Label ID="lblTicketPromedio" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
            </div>
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiVentasEsteMes" runat="server" Text="Ventas Este Mes" /></span>
                <asp:Label ID="lblVentasEsteMes" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
        </div>

        <div class="tarjeta-chasis filtros-panel">
            <div class="filtros-grid">
                <div class="filtro-grupo busqueda-principal">
                    <label><asp:Label ID="lblFiltroBuscarVenta" runat="server" Text="Buscar venta" /></label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="input-taller" placeholder="N° Venta, cliente o vehículo..."></asp:TextBox>
                </div>
                <div class="filtro-grupo">
                    <label><asp:Label ID="lblFiltroFechaDesde" runat="server" Text="Fecha Desde" /></label>
                    <asp:TextBox ID="txtFechaDesde" runat="server" CssClass="input-taller" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filtro-grupo">
                    <label><asp:Label ID="lblFiltroFechaHasta" runat="server" Text="Fecha Hasta" /></label>
                    <asp:TextBox ID="txtFechaHasta" runat="server" CssClass="input-taller" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filtro-grupo acciones-filtro">
                    <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn-primario" OnClick="btnFiltrar_Click" />
                </div>
            </div>
        </div>

        <div class="tarjeta-chasis panel-tabla">
            <div class="tabla-contenedor">
                <asp:GridView ID="dgvVentas" runat="server" AutoGenerateColumns="False" CssClass="gridview-custom">
                    <Columns>
                        <asp:BoundField DataField="NumeroVenta" HeaderText="col_VentaNumero" ItemStyle-Width="12%" HeaderStyle-CssClass="col-header" />
                        
                        <asp:TemplateField HeaderText="col_VentaCliente" ItemStyle-Width="23%">
                            <ItemTemplate>
                                <%# Eval("Cliente.Nombre") %> <%# Eval("Cliente.Apellido") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="col_VentaVehiculoBase" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <%# Eval("AutoBase.Marca") %> <%# Eval("AutoBase.Modelo") %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="col_VentaPersonalizacion" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <span class="badge-custom"><%# Eval("AutoPersonalizado.NombreExtra") ?? Traducir("texto_estandar_base") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Fecha" HeaderText="col_VentaFecha" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="IVA" HeaderText="col_VentaIva" DataFormatString="{0}%" ItemStyle-Width="6%" />
                        
                        <asp:BoundField DataField="Total" HeaderText="col_VentaTotal" DataFormatString="US$ {0:N2}" ItemStyle-Width="12%" ItemStyle-CssClass="col-total" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

    </div>
</asp:Content>
