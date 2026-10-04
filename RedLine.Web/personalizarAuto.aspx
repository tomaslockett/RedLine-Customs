<%@ Page Title="Personalizar Auto" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="personalizarAuto.aspx.cs" Inherits="RedLine.Web.personalizarAuto" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="/Content/PersonalizarAuto.css" rel="stylesheet" type="text/css" runat="server" />
    
    <main class="personalizarAutoContainer">
        <section>
            <article class="autoExpandido">
                <h4 class="marca" id="lblMarca" runat="server"></h4>
                <h2 class="modelo" id="lblModelo" runat="server"></h2>
                <h3 class="anio" id="lblAnio" runat="server"></h3>
                <img src="Content/img/porsche.jfif" alt="Auto" id="img" runat="server" class="imagenAutoExpandido"/>
                <h2 class="precio" id="lblPrecio" runat="server">$</h2>
            </article>

            <article class="personalizar">
                <ul class="listaPersonalizacion">

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="aleron">
                            <asp:Label ID="lblExtraAleron" runat="server" Text="Alerón deportivo" />
                        </label>
                        <span class="precioExtra">+ $500</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="kitCarroceria">
                            <asp:Label ID="lblExtraKit" runat="server" Text="Kit de carrocería" />
                        </label>
                        <span class="precioExtra">+ $1200</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="llantas">
                            <asp:Label ID="lblExtraLlantas" runat="server" Text="Llantas personalizadas" />
                        </label>
                        <span class="precioExtra">+ $2500</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="suspension">
                            <asp:Label ID="lblExtraSuspension" runat="server" Text="Suspensión ajustable" />
                        </label>
                        <span class="precioExtra">+ $3000</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="pintura">
                            <asp:Label ID="lblExtraPintura" runat="server" Text="Pintura personalizada" />
                        </label>
                        <span class="precioExtra">+ $1800</span>
                    </li>

                </ul>

                <asp:Button ID="btnConfirmar" runat="server" CssClass="btnConfirmar" Text="Confirmar configuración" OnClick="btnConfirmar_Click" />
            </article>
        </section>
    </main>
</asp:Content>