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

        <!-- ================= INFORME MENSUAL DE VENTAS ================= -->
        <asp:PlaceHolder ID="phInformeMensual" runat="server">
            <div class="tarjeta-chasis informe-panel">

                <div class="informe-encabezado">
                    <h2 class="informe-titulo"><asp:Label ID="lblTituloInformeMensual" runat="server" Text="Informe Mensual de Ventas" /></h2>
                    <p class="informe-subtitulo"><asp:Label ID="lblSubtituloInformeMensual" runat="server" Text="Análisis estadístico del período seleccionado." /></p>
                </div>

                <div class="informe-selectores">
                    <div class="informe-selector">
                        <label><asp:Label ID="lblInformeMes" runat="server" Text="Mes:" /></label>
                        <asp:DropDownList ID="ddlInformeMes" runat="server" CssClass="input-taller informe-ddl"></asp:DropDownList>
                    </div>
                    <div class="informe-selector">
                        <label><asp:Label ID="lblInformeAnio" runat="server" Text="Año:" /></label>
                        <asp:DropDownList ID="ddlInformeAnio" runat="server" CssClass="input-taller informe-ddl"></asp:DropDownList>
                    </div>
                    <div class="informe-acciones">
                        <asp:Button ID="btnGenerarInforme" runat="server" Text="Generar Informe" CssClass="btn-primario informe-btn" OnClick="btnGenerarInforme_Click" />
                        <asp:Button ID="btnVerInformePdf" runat="server" Text="Ver Informe (PDF)" CssClass="btn-secundario informe-btn" OnClick="btnVerInformePdf_Click" />
                        <asp:Button ID="btnDescargarInformePdf" runat="server" Text="Descargar Informe (PDF)" CssClass="btn-primario informe-btn" OnClick="btnDescargarInformePdf_Click" />
                    </div>
                </div>

                <asp:PlaceHolder ID="phInformeDatos" runat="server" Visible="false">
                    <!-- Encabezado corporativo del informe -->
                    <div class="informe-corporativo">
                        <div class="informe-empresa">
                            <span class="informe-marca">REDLINE CUSTOMS</span>
                            <span class="informe-razon">RedLine Customs S.A. &middot; www.redlinecustoms.com</span>
                            <span class="informe-institucional"><asp:Label ID="lblInformeEmpresaSubtitulo" runat="server" Text="Concesionaria y Taller de Personalización de Alta Gama" /></span>
                            <span class="informe-fiscal">CUIT 30-71829384-9 &middot; Av. del Libertador 4500, CABA</span>
                        </div>
                        <div class="informe-periodo">
                            <span><asp:Label ID="lblInformePeriodo" runat="server" Text="Período:" /> <strong><asp:Label ID="lblInformeValorPeriodo" runat="server" /></strong></span>
                            <span><asp:Label ID="lblInformeFechaEmision" runat="server" Text="Fecha de emisión:" /> <strong><asp:Label ID="lblInformeValorFecha" runat="server" /></strong></span>
                        </div>
                    </div>

                    <!-- KPIs del período -->
                    <div class="kpi-contenedor informe-kpis">
                        <div class="kpi-tarjeta">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiUnidadesMes" runat="server" Text="Unidades Vendidas" /></span>
                            <asp:Label ID="lblValorUnidadesMes" runat="server" CssClass="kpi-valor">0</asp:Label>
                        </div>
                        <div class="kpi-tarjeta destacados">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiFacturacionMes" runat="server" Text="Facturación Total (IVA incl.)" /></span>
                            <asp:Label ID="lblValorFacturacionMes" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
                        </div>
                        <div class="kpi-tarjeta">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiTicketPromedioMes" runat="server" Text="Ticket Promedio" /></span>
                            <asp:Label ID="lblValorTicketPromedioMes" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
                        </div>
                        <div class="kpi-tarjeta">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiMarcaDestacadaMes" runat="server" Text="Marca / Modelo Destacado" /></span>
                            <asp:Label ID="lblValorMarcaDestacadaMes" runat="server" CssClass="kpi-valor kpi-valor-compacto">-</asp:Label>
                        </div>
                        <div class="kpi-tarjeta">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiSubtotalNetoMes" runat="server" Text="Subtotal Neto" /></span>
                            <asp:Label ID="lblValorSubtotalNetoMes" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
                        </div>
                        <div class="kpi-tarjeta">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiIvaMes" runat="server" Text="IVA del Período" /></span>
                            <asp:Label ID="lblValorIvaMes" runat="server" CssClass="kpi-valor">US$ 0,00</asp:Label>
                        </div>
                        <div class="kpi-tarjeta">
                            <span class="kpi-etiqueta"><asp:Label ID="lblKpiPersonalizadosMes" runat="server" Text="Personalizados vs. Estándar" /></span>
                            <asp:Label ID="lblValorPersonalizadosMes" runat="server" CssClass="kpi-valor kpi-valor-compacto">0 / 0 (0%)</asp:Label>
                        </div>
                    </div>

                    <!-- Gráficos estadísticos del mes -->
                    <div class="informe-graficos">
                        <div class="informe-grafico ampliado">
                            <div class="informe-grafico-titulo"><asp:Label ID="lblGraficoEvolucionDiaria" runat="server" Text="Evolución Diaria de Ingresos" /></div>
                            <asp:Literal ID="ltGraficoEvolucionDiaria" runat="server" />
                        </div>
                        <div class="informe-grafico">
                            <div class="informe-grafico-titulo"><asp:Label ID="lblGraficoVentasMarca" runat="server" Text="Facturación por Marca / Modelo" /></div>
                            <asp:Literal ID="ltGraficoVentasMarca" runat="server" />
                        </div>
                        <div class="informe-grafico">
                            <div class="informe-grafico-titulo"><asp:Label ID="lblGraficoPersonalizadosEstandar" runat="server" Text="Personalizados vs. Estándar de Fábrica" /></div>
                            <asp:Literal ID="ltGraficoPersonalizadosEstandar" runat="server" />
                        </div>
                        <div class="informe-grafico ampliado">
                            <div class="informe-grafico-titulo"><asp:Label ID="lblGraficoMetodosPago" runat="server" Text="Distribución por Método de Pago" /></div>
                            <asp:Literal ID="ltGraficoMetodosPago" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="phInformeVacio" runat="server" Visible="false">
                    <div class="informe-vacio">
                        <asp:Label ID="lblInformeSinVentasTitulo" runat="server" CssClass="informe-vacio-titulo" Text="Sin ventas en este período" />
                        <asp:Label ID="lblInformeSinVentasTexto" runat="server" CssClass="informe-vacio-texto" Text="No se registraron operaciones en el mes y año seleccionados. Probá con otro período." />
                    </div>
                </asp:PlaceHolder>

            </div>
        </asp:PlaceHolder>

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
