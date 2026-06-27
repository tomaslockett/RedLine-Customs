<%@ Page Title="Inventario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="RedLine.Web.Inventario" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <link href="Content/Inventario.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="inventario-encabezado">
            <h1 class="titulo">Inventario de Vehículos</h1>
            <p class="subTitulo">Lista de vehículos disponibles en el stock</p>
        </div>

        <div class="inventario-button">
            <asp:Button ID="ButtonAgregarVehiculo" runat="server" Text="Agregar Vehículo" OnClick="IrAAgregarAuto" CssClass="btn-principal" />
        </div>
        
        <div class="agregar-vehiculos">   
            <div class="imagen-caja">
                </div>
                 
            <div class="texto-vehiculos">
                <h1 class="titulin">No hay vehículos en el inventario</h1>
                <p class="parafin">Comienza agregando tu primer vehículo al stock</p>
            </div>

            <div class="boton-primerAuto">
                <asp:Button ID="ButtonAgregarPrimerVehiculo" runat="server" Text="+ Agregar Primer Vehículo" OnClick="IrAAgregarAuto" CssClass="btn-secundario" />
            </div>
        </div>

    </div>

</asp:Content>
