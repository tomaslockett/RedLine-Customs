<%@Page Title= "LogIn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="personalizarAuto.aspx.cs" Inherits="RedLine.Web.personalizarAuto" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <main class="personalizarAutoContainer">
         <section>
             <article class="autoExpandido">
                 <h4 class="marca">Porsche</h4>
                 <h2 class="modelo">911 Turbo S</h2>
                 <h3 class="anio">2024</h3>
                 <img src="Content/img/porsche.jfif" alt="Alternate Text" />
                 <h2 class="precio">$230.000</h2>
             </article>
             <article class="personalizar">
                 <ul class="listaPersonalizacion">

            <li class="itemPersonalizar">
                <label>
                    <input type="checkbox" name="extras" value="aleron">
                    Aleron deportivo
                </label>
                <span class="precioExtra">+ $500</span>
            </li>

            <li class="itemPersonalizar">
                <label>
                    <input type="checkbox" name="extras" value="kitCarroceria">
                    Kit de carroceria
                </label>
                <span class="precioExtra">+ $1200</span>
            </li>

            <li class="itemPersonalizar">
                <label>
                    <input type="checkbox" name="extras" value="llantas">
                    Llantas personalizadas
                </label>
                <span class="precioExtra">+ $2500</span>
            </li>

            <li class="itemPersonalizar">
                <label>
                    <input type="checkbox" name="extras" value="suspension">
                    Suspensión ajustable
                </label>
                <span class="precioExtra">+ $3000</span>
            </li>

            <li class="itemPersonalizar">
                <label>
                    <input type="checkbox" name="extras" value="pintura">
                    Pintura personalizada
                </label>
                <span class="precioExtra">+ $1800</span>
            </li>

        </ul>

        <button type="submit" class="btnConfirmar">Confirmar configuración</button>
             </article>
         </section>
    </main>
</asp:Content>

