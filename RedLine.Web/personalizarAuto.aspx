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
                <h2 class="precio" id="lblPrecio" runat="server" ClientIDMode="Static">$</h2>
            </article>

            <article class="personalizar">
                <ul class="listaPersonalizacion">

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="aleron" data-precio="500">
                            <asp:Label ID="lblExtraAleron" runat="server" Text="Alerón deportivo" />
                        </label>
                        <span class="precioExtra">+ $500</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="kitCarroceria" data-precio="1200">
                            <asp:Label ID="lblExtraKit" runat="server" Text="Kit de carrocería" />
                        </label>
                        <span class="precioExtra">+ $1200</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="llantas" data-precio="2500">
                            <asp:Label ID="lblExtraLlantas" runat="server" Text="Llantas personalizadas" />
                        </label>
                        <span class="precioExtra">+ $2500</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="suspension" data-precio="3000">
                            <asp:Label ID="lblExtraSuspension" runat="server" Text="Suspensión ajustable" />
                        </label>
                        <span class="precioExtra">+ $3000</span>
                    </li>

                    <li class="itemPersonalizar">
                        <label>
                            <input type="checkbox" name="extras" value="pintura" data-precio="1800">
                            <asp:Label ID="lblExtraPintura" runat="server" Text="Pintura personalizada" />
                        </label>
                        <span class="precioExtra">+ $1800</span>
                    </li>

                </ul>

                <asp:Button ID="btnConfirmar" runat="server" CssClass="btnConfirmar" Text="Confirmar configuración" OnClick="btnConfirmar_Click" />
            </article>
        </section>
    </main>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            var lblPrecio = document.getElementById('lblPrecio');
            if (!lblPrecio) return;

            var precioBaseTexto = lblPrecio.getAttribute('data-precio-base');
            var precioBase = parseFloat(precioBaseTexto);
            if (isNaN(precioBase)) return;

            var checkboxes = document.querySelectorAll('input[name="extras"]');

            function actualizarPrecioTotal() {
                var total = precioBase;
                for (var i = 0; i < checkboxes.length; i++) {
                    if (checkboxes[i].checked) {
                        var extraPrecio = parseFloat(checkboxes[i].getAttribute('data-precio') || '0');
                        if (!isNaN(extraPrecio)) {
                            total += extraPrecio;
                        }
                    }
                }
                lblPrecio.innerText = '$ ' + total.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }

            for (var j = 0; j < checkboxes.length; j++) {
                checkboxes[j].addEventListener('change', actualizarPrecioTotal);
            }
        });
    </script>
</asp:Content>
