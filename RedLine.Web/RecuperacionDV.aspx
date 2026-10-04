<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecuperacionDV.aspx.cs" Inherits="RedLine.Web.RecuperacionDV" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Recuperación de Dígito Verificador</title>
    <link href="/Content/RecuperacionDV.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="contenedor-eventos-main">
            <div class="panel-bitacora-recuadro panel-inconsistencias">
                
                <h2 class="titulo-bitacora">
                    <asp:Label ID="lblTituloPanelInconsistencias" runat="server" Text="Panel de Inconsistencias" />
                </h2>
                
                <div class="grupo-acciones-inconsistencias">
                    <asp:Button ID="btnRecalcular" runat="server" Text="Recalcular el DV" CssClass="boton-bitacora-accion" OnClick="RecalcularDV" />
                    <asp:Button ID="btnRestore" runat="server" Text="Restore de BD" CssClass="boton-bitacora-accion" OnClick="RestoreDV" />
                    <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="boton-bitacora-accion boton-salir" OnClick="SalirDV" />
                </div>

                <div class="seccion-reporte">
                    <span class="etiqueta-reporte">
                        <asp:Label ID="lblEtiquetaReporte" runat="server" Text="Inconsistencias en las tablas:" />
                    </span>
                    <div class="cuadro-log-inconsistencias">
                        <p class="log-vacio" id="log" runat="server">
                            <asp:Label ID="lblLogVacio" runat="server" Text="No se detectaron inconsistencias actuales." />
                        </p>
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>