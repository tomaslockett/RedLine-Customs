<%@ Page Title="Inventario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="RedLine.Web.Inventario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Inventario.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="inventario-encabezado">
            <div class="info-encabezado">
                <h1 class="titulo">Inventario de Vehículos</h1>
                <p class="subTitulo">Lista de vehículos disponibles en el stock</p>
            </div>
            <div class="inventario-button">
                <asp:Button ID="ButtonAgregarVehiculo" runat="server" Text="Agregar Vehículo" OnClick="IrAAgregarAuto" CssClass="btn btn-primario" />
            </div>
        </div>
        
       <asp:PlaceHolder ID="phInventarioVacio" runat="server" Visible="false">
    <div class="agregar-vehiculos">   
        <div class="imagen-caja"></div>
        <div class="texto-vehiculos">
            <h1 class="titulin">No hay vehículos en el inventario</h1>
            <p class="parafin">Comienza agregando tu primer vehículo al stock</p>
        </div>
        <div class="boton-primerAuto">
            <asp:Button ID="ButtonAgregarPrimerVehiculo" runat="server" Text="+ Agregar Primer Vehículo" OnClick="IrAAgregarAuto" CssClass="btn btn-secundario" />
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="phInventarioLista" runat="server" Visible="true">
    <div class="tabla-inventario">
        <asp:Repeater ID="repInventario" runat="server">
            <ItemTemplate>
                <div class="fila-vehiculo">
                    <div class="col-foto">
                        <img src='<%# "data:image/jpeg;base64," + Convert.ToBase64String((byte[])Eval("ImagenBinaria")) %>' alt="Foto Vehículo" />
                    </div>
                    <div class="col-detalles">
                        <span class="marca-stock"><%# Eval("Marca") %></span>
                        <h3 class="modelo-stock"><%# Eval("Modelo") %> (<%# Eval("Anio") %>)</h3>
                        <div class="mini-specs">
                            <span>V. Máx: <%# Eval("VelocidadMaxima") %> km/h</span>
                            <span>Potencia: <%# Eval("Potencia") %> HP</span>
                        </div>
                    </div>
                    <div class="col-precio">
                        <span class="precio-stock">$ <%# Eval("PrecioBase") %></span>
                    </div>
                    <div class="col-acciones">
                        <a href='personalizarAuto.aspx?id=<%# Eval("ID") %>' class="btn-editar">Editar</a>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>
    </div>
</asp:Content>