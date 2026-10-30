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
                <span id="lblPrecioBaseHidden" style="display:none;" runat="server">0</span>
            </article>

            <article class="personalizar">
                <ul class="listaPersonalizacion">

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="aleron" data-precio="500" class="chk-extra">
                            <asp:Label ID="lblExtraAleron" runat="server" Text="Alerón deportivo" />
                        </label>
                        <span class="precioExtra">+ $500</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="kitCarroceria" data-precio="1200" class="chk-extra">
                            <asp:Label ID="lblExtraKit" runat="server" Text="Kit de carrocería" />
                        </label>
                        <span class="precioExtra">+ $1,200</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="llantas" data-precio="2500" class="chk-extra">
                            <asp:Label ID="lblExtraLlantas" runat="server" Text="Llantas personalizadas" />
                        </label>
                        <span class="precioExtra">+ $2,500</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="suspension" data-precio="3000" class="chk-extra">
                            <asp:Label ID="lblExtraSuspension" runat="server" Text="Suspensión ajustable" />
                        </label>
                        <span class="precioExtra">+ $3,000</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="pintura" data-precio="1800" class="chk-extra">
                            <asp:Label ID="lblExtraPintura" runat="server" Text="Pintura personalizada" />
                        </label>
                        <span class="precioExtra">+ $1,800</span>
                    </li>

                </ul>

                <asp:HiddenField ID="hfAutoId" runat="server" />
                <asp:Button ID="btnConfirmar" runat="server" CssClass="btnConfirmar" Text="Confirmar configuración e ir al Checkout" OnClick="btnConfirmar_Click" />
            </article>
        </section>
    </main>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const lblPrecio = document.getElementById('<%= lblPrecio.ClientID %>');
            const lblPrecioBaseHidden = document.getElementById('<%= lblPrecioBaseHidden.ClientID %>');
            const checkboxes = document.querySelectorAll('.chk-extra');

            const basePrice = parseFloat(lblPrecioBaseHidden ? lblPrecioBaseHidden.innerText : 0) || 0;

            function actualizarPrecioTotal() {
                let extraTotal = 0;
                checkboxes.forEach(function (chk) {
                    if (chk.checked) {
                        extraTotal += parseFloat(chk.getAttribute('data-precio') || 0);
                    }
                });

                const total = basePrice + extraTotal;
                if (lblPrecio) {
                    lblPrecio.innerText = '$ ' + total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                }
            }

            checkboxes.forEach(function (chk) {
                chk.addEventListener('change', actualizarPrecioTotal);
            });
        });
    </script>
</asp:Content>
