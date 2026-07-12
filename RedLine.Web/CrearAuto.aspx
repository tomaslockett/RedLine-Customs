<%@ Page Title="Creacion de Auto" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CrearAuto.aspx.cs" Inherits="RedLine.Web.CrearAuto" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="Content/CrearAutosEstilo.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-principal">
        
        <div class="volver-inventario">
            <a href="Inventario.aspx">← Volver al Inventario</a>
        </div>

        <div class="encabezado-pagina">
            <h1 class="titulo">Agregar Nuevo Vehículo al Stock</h1>
            <p class="subTitulo">Complete el formulario para añadir un vehículo al inventario</p>
        </div>

        <div class="tarjeta">
            <h2 class="tarjeta-titulo">Imagen del Vehículo</h2>
            <div class="zona-subida">
                <p>Seleccione la imagen oficial del vehículo</p>
                <div class="contenedor-file">
                    <asp:FileUpload ID="FileUploadSubirFoto" runat="server" accept="image/*" CssClass="file-upload" />
                </div>
                <p class="formato-texto">Formatos admitidos: JPG, PNG, WEBP</p>
            </div>
        </div>

        <div class="tarjeta">
            <h2 class="tarjeta-titulo">Datos Base del Vehículo</h2>
            
            <div class="grid-formulario">
                <div class="grupo-form">
                    <label>ID del Auto *</label>
                    <asp:TextBox ID="TextBoxID" runat="server" CssClass="input-control" placeholder="ej: VEH-2026-001"></asp:TextBox>
                </div>

                <div class="grupo-form">
                    <label>Marca *</label>
                    <asp:DropDownList ID="DropDownListMarca" runat="server" CssClass="input-control">
                        <asp:ListItem Text="Seleccione una marca" Value=""></asp:ListItem>
                        <asp:ListItem Text="Alfa Romeo" Value="Alfa Romeo"></asp:ListItem>
                        <asp:ListItem Text="Audi" Value="Audi"></asp:ListItem>
                        <asp:ListItem Text="BMW" Value="BMW"></asp:ListItem>
                        <asp:ListItem Text="BYD" Value="BYD"></asp:ListItem>
                        <asp:ListItem Text="Chevrolet" Value="Chevrolet"></asp:ListItem>
                        <asp:ListItem Text="Citroën" Value="Citroën"></asp:ListItem>
                        <asp:ListItem Text="Chery" Value="Chery"></asp:ListItem>
                        <asp:ListItem Text="Chrysler" Value="Chrysler"></asp:ListItem>
                        <asp:ListItem Text="Dodge" Value="Dodge"></asp:ListItem>
                        <asp:ListItem Text="Ferrari" Value="Ferrari"></asp:ListItem>
                        <asp:ListItem Text="Fiat" Value="Fiat"></asp:ListItem>
                        <asp:ListItem Text="Ford" Value="Ford"></asp:ListItem>
                        <asp:ListItem Text="Geely" Value="Geely"></asp:ListItem>
                        <asp:ListItem Text="Honda" Value="Honda"></asp:ListItem>
                        <asp:ListItem Text="Hyundai" Value="Hyundai"></asp:ListItem>
                        <asp:ListItem Text="Jeep" Value="Jeep"></asp:ListItem>
                        <asp:ListItem Text="Kia" Value="Kia"></asp:ListItem>
                        <asp:ListItem Text="Lamborghini" Value="Lamborghini"></asp:ListItem>
                        <asp:ListItem Text="Land Rover" Value="Land Rover"></asp:ListItem>
                        <asp:ListItem Text="Lexus" Value="Lexus"></asp:ListItem>
                        <asp:ListItem Text="Maserati" Value="Maserati"></asp:ListItem>
                        <asp:ListItem Text="Mazda" Value="Mazda"></asp:ListItem>
                        <asp:ListItem Text="Mercedes-Benz" Value="Mercedes-Benz"></asp:ListItem>
                        <asp:ListItem Text="MINI" Value="MINI"></asp:ListItem>
                        <asp:ListItem Text="Mitsubishi" Value="Mitsubishi"></asp:ListItem>
                        <asp:ListItem Text="Nissan" Value="Nissan"></asp:ListItem>
                        <asp:ListItem Text="Peugeot" Value="Peugeot"></asp:ListItem>
                        <asp:ListItem Text="Porsche" Value="Porsche"></asp:ListItem>
                        <asp:ListItem Text="RAM" Value="RAM"></asp:ListItem>
                        <asp:ListItem Text="Renault" Value="Renault"></asp:ListItem>
                        <asp:ListItem Text="Subaru" Value="Subaru"></asp:ListItem>
                        <asp:ListItem Text="Suzuki" Value="Suzuki"></asp:ListItem>
                        <asp:ListItem Text="Tesla" Value="Tesla"></asp:ListItem>
                        <asp:ListItem Text="Toyota" Value="Toyota"></asp:ListItem>
                        <asp:ListItem Text="Volkswagen" Value="Volkswagen"></asp:ListItem>
                        <asp:ListItem Text="Volvo" Value="Volvo"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="grupo-form">
                    <label>Modelo *</label>
                    <asp:TextBox ID="TextBoxModelo" runat="server" CssClass="input-control" placeholder="ej: 911 GT3 RS"></asp:TextBox>
                </div>

                <div class="grupo-form">
                    <label>Año *</label>
                    <asp:TextBox ID="TextBoxAño" runat="server" CssClass="input-control" placeholder="ej: 2026"></asp:TextBox>
                </div>

                <div class="grupo-form">
                    <label>Precio Base (USD) *</label>
                    <asp:TextBox ID="TextBoxPrecio" runat="server" CssClass="input-control" TextMode="Number" placeholder="ej: 225000"></asp:TextBox>
                </div>

                <div class="grupo-form">
                    <label>Tipo *</label>
                    <asp:TextBox ID="TextBoxTipo" runat="server" CssClass="input-control" placeholder="ej: Deportivo / Coupé"></asp:TextBox>
                </div>

                <div class="grupo-form">
                    <label>Potencia (CV)</label>
                    <asp:TextBox ID="TextBoxPotencia" runat="server" CssClass="input-control" placeholder="ej: 525"></asp:TextBox>
                </div>

                <div class="grupo-form">
                    <label>Velocidad Máxima (Km/h)</label>
                    <asp:TextBox ID="TextBoxVelocidadMaxima" runat="server" CssClass="input-control" placeholder="ej: 296"></asp:TextBox>
                </div>
                
                <div class="grupo-form ancho-completo">
                    <label>Aceleración 0-100 Km/h (Segundos)</label>
                    <asp:TextBox ID="TextBoxAceleracion" runat="server" CssClass="input-control" placeholder="ej: 3.2"></asp:TextBox>
                </div>

                <div class="grupo-form ancho-completo">
                    <label>Descripción General</label>
                    <asp:TextBox ID="TextBoxDescripcionGeneral" runat="server" CssClass="input-control" TextMode="MultiLine" Rows="4" placeholder="Detalles de ingeniería o equipamiento de fábrica..."></asp:TextBox>
                </div>
            </div>

            <div class="acciones-formulario">
                <asp:Button ID="ButtonCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secundario" OnClick="Cancelar"/>
                <asp:Button ID="ButtonGuarda" runat="server" Text="Guardar Vehículo" CssClass="btn btn-primario" OnClick="Agregar"/>
            </div>
        </div>

    </div>


</asp:Content>
