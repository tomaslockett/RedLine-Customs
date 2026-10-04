<%@ Page Title="Gestión De Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="RedLine.Web.GestionClientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/GestionClientes.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="encabezado-pagina">
            <h1 class="titulo"><asp:Label ID="lblTituloGestionClientes" runat="server" Text="Consulta de Clientes" /></h1>
            <p class="subTitulo"><asp:Label ID="lblSubtituloGestionClientes" runat="server" Text="Administración del registro de clientes activos y auditoría de cuentas." /></p>
        </div>

        <div class="kpi-contenedor">
            <div class="kpi-tarjeta destacados">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiTotalClientes" runat="server" Text="Total Clientes" /></span>
                <asp:Label ID="lblTotalClientes" runat="server" CssClass="kpi-valor">12</asp:Label>
            </div>
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiActivosMes" runat="server" Text="Activos Este Mes" /></span>
                <asp:Label ID="lblActivosMes" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
            <div class="kpi-tarjeta">
                <span class="kpi-etiqueta"><asp:Label ID="lblKpiNuevosMes" runat="server" Text="Nuevos Este Mes" /></span>
                <asp:Label ID="lblNuevosMes" runat="server" CssClass="kpi-valor">0</asp:Label>
            </div>
        </div>

        <div class="tarjeta-chasis panel-importacion" style="margin-bottom: 20px; padding: 15px;">
            <h3><asp:Label ID="lblTituloImportacion" runat="server" Text="Importación Masiva de Clientes" /></h3>
            <p><asp:Label ID="lblSubtituloImportacion" runat="server" Text="Seleccione un archivo formato XML para incorporar nuevos registros." /></p>
            
            <div style="display: flex; gap: 10px; align-items: center; margin-top: 10px;">
                <asp:FileUpload ID="fuClientesXml" runat="server" CssClass="form-control" />
                <asp:Button ID="btnImportarXml" runat="server" Text="Cargar XML" CssClass="btn-link-ver" OnClick="btnImportarXml_Click" style="padding: 8px 15px; cursor: pointer;" />
            </div>

            <asp:Label ID="lblMensajeImportacion" runat="server" Visible="false" Style="display: block; margin-top: 10px; font-weight: bold;"></asp:Label>
        </div>

        <div class="tarjeta-chasis panel-tabla">
            <div class="tabla-contenedor">
                <asp:GridView ID="dgvClientes" runat="server" AutoGenerateColumns="False" CssClass="gridview-custom" OnRowCommand="dgvClientes_RowCommand" OnSelectedIndexChanged="dgvClientes_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderText="col_ClienteId" ItemStyle-Width="10%">
                            <ItemTemplate>
                                <span class="badge-id">USR-<%# (Container.DataItemIndex + 1).ToString("D4") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="DNI" HeaderText="col_ClienteDni" ItemStyle-Width="10%" />
                        <asp:BoundField DataField="Nombre" HeaderText="col_ClienteNombre" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="Apellido" HeaderText="col_ClienteApellido" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="Email" HeaderText="col_ClienteEmail" ItemStyle-Width="20%" />
                        <asp:BoundField DataField="Telefono" HeaderText="col_ClienteTelefono" ItemStyle-Width="12%" />
                        <asp:BoundField DataField="Direccion" HeaderText="col_ClienteDireccion" ItemStyle-Width="18%" />
                        
                        <asp:TemplateField HeaderText="col_ClientePassword" ItemStyle-Width="8%">
                            <ItemTemplate>
                                <span class="contrasena-oculta">••••••••</span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="col_ClienteAcciones" ItemStyle-Width="8%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnVer" runat="server" CssClass="btn-link-ver" CommandName="VerPerfil" CommandArgument='<%# Eval("DNI") %>'>
                                    <%# Traducir("btn_ver_perfil_cliente") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            
            <div class="tabla-footer-resumen">
                <asp:Label ID="lblResumenPaginacion" runat="server" Text="Mostrando clientes"></asp:Label>
            </div>
        </div>

    </div>
</asp:Content>