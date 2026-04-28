<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearAutos.aspx.cs" Inherits="RedLine.Web.CrearAutos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Agregar Nuevo Auto</title>
    <link href="/Content/CrearAutosEstilo.css" rel="stylesheet" type="text/css" runat="server" />

</head>
<body>
   <form id="form1" runat="server">
        <div class="contenedor-principal">
            
            <div class="volver-inventario">
                <a href="#">← Volver al Inventario</a>
            </div>

            <div class="encabezado-pagina">
                <h1 class="titulo">Agregar Nuevo Vehículo al Stock</h1>
                <p class="subTitulo">Complete el formulario para añadir un vehículo al inventario</p>
            </div>

            <div class="tarjeta">
                <h2 class="tarjeta-titulo">Imagen del Vehículo</h2>
                
                <div class="zona-subida">
                    <p>Arrastra y suelta una imagen aquí</p>
                    
                    <div class="contenedor-file">
                        <asp:FileUpload ID="FileUploadSubirFoto" runat="server" accept="image/*" CssClass="file-upload" />
                    </div>
                    
                    <p class="formato-texto">PNG</p>
                </div>
            </div>

            <div class="tarjeta">
                <h2 class="tarjeta-titulo">Datos Base del Vehículo</h2>
                
                <div class="grid-formulario">
                    <div class="grupo-form">
                        <label>ID del Auto *</label>
                        <asp:TextBox ID="TextBoxID" runat="server" CssClass="input-control" placeholder="ej: VEH-2024-001"></asp:TextBox>
                    </div>
                    <div class="grupo-form">
                        <label>Marca *</label>
                        <asp:DropDownList ID="DropDownListMarca" runat="server" CssClass="input-control">
                            <asp:ListItem Text="Seleccione una marca" Value=""></asp:ListItem>
                            <asp:ListItem Text="Toyota" Value="Toyota"></asp:ListItem>
                            <asp:ListItem Text="Ford" Value="Ford"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="grupo-form">
                        <label>Modelo *</label>
                        <asp:TextBox ID="TextBoxModelo" runat="server" CssClass="input-control"></asp:TextBox>
                    </div>
                    <div class="grupo-form">
                        <label>Año *</label>
                        <asp:TextBox ID="TextBoxAño" runat="server" CssClass="input-control"></asp:TextBox>
                    </div>

                    <div class="grupo-form">
                        <label>Precio Base (USD) *</label>
                        <asp:TextBox ID="TextBoxPrecio" runat="server" CssClass="input-control" TextMode="Number"></asp:TextBox>
                    </div>
                    <div class="grupo-form">
                        <label>Tipo *</label>
                        <asp:TextBox ID="TextBoxTipo" runat="server" CssClass="input-control"></asp:TextBox>
                    </div>

                    <div class="grupo-form">
                        <label>Potencia</label>
                        <asp:TextBox ID="TextBoxPotencia" runat="server" CssClass="input-control"></asp:TextBox>
                    </div>
                    <div class="grupo-form">
                        <label>Velocidad Máxima</label>
                        <asp:TextBox ID="TextBoxVelocidadMaxima" runat="server" CssClass="input-control"></asp:TextBox>
                    </div>
                    
                    <div class="grupo-form ancho-completo">
                        <label>Aceleración 0-100Km/h</label>
                        <asp:TextBox ID="TextBoxAceleracion" runat="server" CssClass="input-control"></asp:TextBox>
                    </div>

                    <div class="grupo-form ancho-completo">
                        <label>Descripción General</label>
                        <asp:TextBox ID="TextBoxDescripcionGeneral" runat="server" CssClass="input-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                    </div>
                </div>

                <div class="acciones-formulario">
                    <asp:Button ID="ButtonCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secundario" />
                    <asp:Button ID="ButtonGuarda" runat="server" Text="Guardar" CssClass="btn btn-primario" />
                </div>
            </div>

        </div>
    </form>
</body>
</html>
