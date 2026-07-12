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
                
                <h2 class="titulo-bitacora">Panel de Inconsistencias</h2>
                
                <div class="grupo-acciones-inconsistencias">

                    <button class="boton-bitacora-accion" id="recalcular" runat="server" OnServerClick="RecalcularDV">Recalcular el DV</button>
                    <button class="boton-bitacora-accion" id="restore" runat="server" OnServerClick="RestoreDV">Restore de BD</button>
                    <button class="boton-bitacora-accion boton-salir" id="salir" runat="server" OnServerClick="SalirDV">Salir</button>
                </div>

                <div class="seccion-reporte">
                    <span class="etiqueta-reporte">Inconsistencias en las tablas:</span>
                    <div class="cuadro-log-inconsistencias">
                        <p class="log-vacio" id="log" runat="server">No se detectaron inconsistencias actuales.</p>
                    </div>
                </div>

            </div>
        </div>

    </form>
</body>
</html>
