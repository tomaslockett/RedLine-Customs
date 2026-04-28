<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RedLine.Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
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
   <div class="auto">
    
    <img class="fotoAuto" src="Content/img/porsche.jfif"/>

    <div class="contenidoAuto">
        <h4 class="marca">Porsche</h4>
        <h2 class="modelo">911 Turbo S</h2>
        <h3 class="anio">2024</h3>

        <div class="datos">
            <div class="datosAuto">
                <p class="lblDatosAuto">Vel. max</p>
                330 km/h</div>
            <div class="datosAuto"><p class="lblDatosAuto">Potencia</p>640 HP</div>
            <div class="datosAuto"><p class="lblDatosAuto">0-100km/h</p> 2.7s</div>
        </div>

        <h2 class="precio">$230.000</h2>
        <div class="botonBajo">Personalizar</div>
    </div>

</div>
</asp:Content>
