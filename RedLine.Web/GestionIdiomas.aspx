<%@ Page Title="Gestión de Idiomas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionIdiomas.aspx.cs" Inherits="RedLine.Web.GestionIdiomas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/GestionIdiomas.css" rel="stylesheet" type="text/css" />

    <main class="gestion-idiomas-container">
        <h1><asp:Label ID="lblTituloPagina" runat="server" Text="Gestión de Idiomas y Traducciones" /></h1>

        <div style="margin-bottom: 15px;">
            <asp:Label ID="lblEstado" runat="server" Font-Bold="true"></asp:Label>
        </div>

        <section class="seccion-card">
            <h2><asp:Label ID="lblSubtituloNuevoIdioma" runat="server" Text="Crear Nuevo Idioma" /></h2>
            <div class="form-inline">
                <asp:TextBox ID="txtNombreIdioma" runat="server" CssClass="input-text" Placeholder="Nombre del idioma (ej: Portugués)" Width="300px" />
                <asp:Button ID="btnCrearIdioma" runat="server" Text="Guardar Idioma" CssClass="btn-rojo" OnClick="BtnCrearIdioma_Click" />
            </div>
        </section>

        <section class="seccion-card">
            <h2><asp:Label ID="lblSubtituloTraducciones" runat="server" Text="Editar Traducciones" /></h2>
            
            <div class="form-inline" style="margin-bottom: 20px;">
                <label><asp:Label ID="lblSeleccionarIdioma" runat="server" Text="Idioma a traducir:" /></label>
                <asp:DropDownList ID="ddlIdiomaDestino" runat="server" CssClass="input-text" AutoPostBack="true" OnSelectedIndexChanged="DdlIdiomaDestino_SelectedIndexChanged" />
                
                <label style="margin-left: 20px;"><asp:Label ID="lblBuscar" runat="server" Text="Buscar etiqueta:" /></label>
                <asp:TextBox ID="txtFiltro" runat="server" CssClass="input-text" AutoPostBack="true" OnTextChanged="TxtFiltro_TextChanged" Placeholder="Filtrar por clave..." />
            </div>

            <asp:GridView ID="gvTraducciones" runat="server" AutoGenerateColumns="false" CssClass="grid-traducciones" DataKeyNames="EtiquetaKey">
                <Columns>
                    <asp:BoundField DataField="EtiquetaKey" HeaderText="Clave de Etiqueta" ReadOnly="true" ItemStyle-Width="30%" />
                    <asp:TemplateField HeaderText="Traducción">
                        <ItemTemplate>
                            <asp:TextBox ID="txtTextoTraducido" runat="server" Text='<%# Bind("Texto") %>' CssClass="input-text" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div style="margin-top: 20px; text-align: right;">
                <asp:Button ID="btnGuardarTraducciones" runat="server" Text="Guardar Todas las Traducciones" CssClass="btn-rojo" OnClick="BtnGuardarTraducciones_Click" />
            </div>
        </section>
    </main>
</asp:Content>