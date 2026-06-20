<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Catalogo.aspx.cs" Inherits="RedLine.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Catalogo.css" rel="stylesheet" type="text/css" />
    
    <div class ="divsRojosGradiente">
        <h1 class = "titulo">Catalogo de autos deportivos</h1>
        <h2 class="subTitulo">Descubra nuestra seleccion de autos de alta gamma</h2>
    </div>

    <div class="buscador">
        <img class="icono" src="Content/img/lupita.png"/>
        <input type="text" placeholder="Buscar por modelo o marca..." class="inputBarraBusqueda"/>
    </div>

    <div class="filtros">
    <select class="filtro">
        <option>Marca</option>
    </select>

    <select class="filtro">
        <option>Precio</option>
    </select>

    <button class="btnFiltro">Aplicar</button>
    </div>
    <br />
    <div>
        <h2 id="lblAutosEncontrados" class ="textoEncontrados">Se encontraron x autos</h2>
    </div>
    <br />
    <asp:Repeater runat="server" ID="repAutos">
        <ItemTemplate>
               <div class="auto" >
    
               <img class="fotoAuto" src='<%# Eval("ImagenUrl") %>'/>

               <div class="contenidoAuto">
                    <h4 class="marca"><%# Eval("Marca") %></h4>
                    <h2 class="modelo"><%# Eval("Modelo") %></h2>
                    <h3 class="anio"><%# Eval("Anio") %></h3>

                    <div class="datos">
                          <div class="datosAuto">
                                <p class="lblDatosAuto">Vel. max</p>
                                <%# Eval("VelocidadMaxima") %>Km/h
                          </div>
                          <div class="datosAuto"><p class="lblDatosAuto">Potencia</p><%# Eval("Potencia") %>HP</div>
                          <div class="datosAuto"><p class="lblDatosAuto">0-100km/h</p> <%# Eval("Aceleracion0a100") %>s</div>
                   </div>

                   <h2 class="precio">$ <%# Eval("PrecioBase") %></h2>
                   <a class="botonBajo" href='personalizarAuto.aspx?id=<%# Eval("ID") %>'>  Personalizar </a>
               </div>

               </div>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>
