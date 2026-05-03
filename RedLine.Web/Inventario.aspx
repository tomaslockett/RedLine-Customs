<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="RedLine.Web.Inventario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
             <div class="contenedor-principal">
            <div class="inventario-encabezado">
                    <h1 class="titulo">Inventario de Vehículos</h1>
                    <p class="subTitulo">Lista de vehículos disponibles en el stock</p>
            </div>
            <div class="inventario-button">
                <asp:Button ID="ButtonAgregarVehiculo" runat="server" Text="Agregar Vehículo" />
            </div>
            <div class="agregar-vehiculos">   
                <div class="imagen-caja">

                </div>
                 
                <div class="texto-vehiculos">
                    <h1 class="titulin">No hay vehiculos en el inventario</h1>
                    <p class="parafin">Comienza Agregando tu primer vehiculo al stock</p>
                </div>
                <div class="boton-primerAuto">
                    <asp:Button ID="ButtonAgregarPrimerVehiculo" runat="server" Text="+ Agregar Primer Vehículo" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
