<%@ Page Title= "LogIn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LogIn.aspx.cs" Inherits="RedLine.Web.LogIn"  %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
     <main class="loginContainer">
        <section class="loginBox">
            
            <h1 class="loginTitulo">Iniciar sesión</h1>

            <div class="campo">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" Class="inputLogin" />
            </div>

            <div class="campo">
                <label>Contraseña</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Class="inputLogin" />
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Ingresar" Class="btnLogin" />

        </section>
    </main>
</asp:Content>
