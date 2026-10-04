<%@ Page Title="Catálogo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Catalogo.aspx.cs" Inherits="RedLine.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Catalogo.css" rel="stylesheet" type="text/css" />
    
    <div class="divsRojosGradiente">
        <h1 class="titulo"><asp:Label ID="lblTituloCatalogo" runat="server" Text="Catálogo de autos deportivos" /></h1>
        <h2 class="subTitulo"><asp:Label ID="lblSubTituloCatalogo" runat="server" Text="Descubra nuestra selección de autos de alta gama" /></h2>
    </div>

    <div class="contenedor-busqueda">
        <div class="buscador">
            <img class="icono" src="Content/img/lupita.png"/>
            <asp:TextBox ID="txtBuscarCatalogo" runat="server" placeholder="Buscar por modelo o marca..." CssClass="inputBarraBusqueda" />
        </div>

        <div class="filtros">
            <asp:DropDownList ID="ddlFiltroMarca" runat="server" CssClass="filtro">
                <asp:ListItem Value="" Text="Marca" />
            </asp:DropDownList>

            <asp:DropDownList ID="ddlFiltroPrecio" runat="server" CssClass="filtro">
                <asp:ListItem Value="" Text="Precio" />
            </asp:DropDownList>

            <asp:Button ID="btnAplicarFiltros" runat="server" Text="Aplicar" CssClass="btnFiltro" OnClick="btnAplicarFiltros_Click" />
        </div>
    </div>

    <br />
    <div>
        <h2 class="textoEncontrados">
            <asp:Label ID="lblTextoEncontrados" runat="server" Text="Autos disponibles en catálogo" />
        </h2>
    </div>
    <br />

    <div class="contenedor-catalogo">
        <asp:Repeater runat="server" ID="repAutos">
            <ItemTemplate>
                <div class="auto">
                    <img class="fotoAuto" src='<%# "data:image/jpeg;base64," + Convert.ToBase64String((byte[])Eval("ImagenBinaria")) %>' />
                    <div class="contenidoAuto">
                        <h4 class="marca"><%# Eval("Marca") %></h4>
                        <h2 class="modelo"><%# Eval("Modelo") %></h2>
                        <h3 class="anio"><%# Eval("Anio") %></h3>

                        <div class="datos">
                            <div class="datosAuto">
                                <p class="lblDatosAuto"><%# Traducir("lblVelMax") %></p>
                                <%# Eval("VelocidadMaxima") %> Km/h
                            </div>
                            <div class="datosAuto">
                                <p class="lblDatosAuto"><%# Traducir("lblPotencia") %></p>
                                <%# Eval("Potencia") %> HP
                            </div>
                            <div class="datosAuto">
                                <p class="lblDatosAuto"><%# Traducir("lblAceleracion") %></p> 
                                <%# Eval("Aceleracion0a100") %> s
                            </div>
                        </div>

                        <h2 class="precio">$ <%# Eval("PrecioBase") %></h2>
                        <a class="botonBajo" href='personalizarAuto.aspx?id=<%# Eval("ID") %>'>
                            <%# Traducir("btnPersonalizar") %>
                        </a>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>