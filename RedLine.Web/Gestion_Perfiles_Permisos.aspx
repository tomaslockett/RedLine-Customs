<%@ Page Title="Gestion de Perfiles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Gestion_Perfiles_Permisos.aspx.cs" Inherits="RedLine.Web.Gestion_Perfiles_Permisos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   <link href="Content/GestionPermisos.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-permisos animate-fade-in">
        
        <header class="encabezado-seccion">
            <div class="header-navegacion">
                <asp:LinkButton ID="btnVolverDashboard" runat="server" CssClass="btn-volver" PostBackUrl="~/AdminDashboard.aspx">
                    <span>←</span> Volver al Dashboard
                </asp:LinkButton>
            </div>
            <h1 class="titulo-principal">Seguridad: Perfiles y Permisos</h1>
            <p class="subtitulo-tecnico">Configuración de roles jerárquicos y asignación de privilegios inmutables del sistema (Patrón Composite).</p>
        </header>

        <div class="split-panel">
            
            <section class="columna-perfiles">
                <div class="tarjeta-glassmorphic">
                    <h2 class="titulo-tarjeta">Perfiles del Sistema</h2>
                    <p class="descripcion-tarjeta">Creá o seleccioná un perfil (Familia Composite) para administrar sus accesos.</p>
                    
                    <div class="formulario-creacion">
                        <asp:TextBox ID="txtNuevoPerfil" runat="server" CssClass="input-premium" placeholder="Nombre del Nuevo Perfil..."></asp:TextBox>
                        <asp:Button ID="btnCrearPerfil" runat="server" Text="Crear Perfil" CssClass="btn-accion-rojo" OnClick="btnCrearPerfil_Click" />
                    </div>

                    <div class="lista-scroll-contenedor">
                        <asp:ListBox ID="lstPerfiles" runat="server" AutoPostBack="True" OnSelectedIndexChanged="lstPerfiles_SelectedIndexChanged" CssClass="listbox-premium">
                        </asp:ListBox>
                    </div>
                    
                    <div class="acciones-perfil-pie">
                        <asp:Button ID="btnRenombrar" runat="server" Text="Renombrar" CssClass="btn-link-gris" />
                        <asp:Button ID="btnEliminarPerfil" runat="server" Text="Eliminar Perfil" CssClass="btn-link-rojo" />
                    </div>
                </div>
            </section>

            <section class="columna-permisos-matriz">
                <div class="tarjeta-glassmorphic status-activa">
                    <h2 class="titulo-tarjeta">
                        Permisos asignados a: <span class="rol-seleccionado-badge"><asp:Label ID="lblPerfilSeleccionado" runat="server" Text="Seleccione un perfil..."></asp:Label></span>
                    </h2>
                    <p class="descripcion-tarjeta info-alerta">Las patentes listadas son inmutables a nivel código. Solo podés activar o desactivar su relación con la familia seleccionada.</p>
                    
                    <div class="matriz-scroll-contenedor">
                        
                        <div class="bloque-modulo">
                            <h3 class="titulo-modulo-interno">Módulo Catálogo e Inventario</h3>
                            <div class="grupo-checkbox-premium">
                                <asp:CheckBoxList ID="cblPermisosCatalogo" runat="server" RepeatLayout="UnorderedList" CssClass="checkboxlist-premium">
                                </asp:CheckBoxList>
                            </div>
                        </div>

                        <div class="bloque-modulo">
                            <h3 class="titulo-modulo-interno">Módulo Comercial y Ventas</h3>
                            <div class="grupo-checkbox-premium">
                                <asp:CheckBoxList ID="cblPermisosVentas" runat="server" RepeatLayout="UnorderedList" CssClass="checkboxlist-premium">
                                </asp:CheckBoxList>
                            </div>
                        </div>

                        <div class="bloque-modulo">
                            <h3 class="titulo-modulo-interno">Módulo Auditoría y Seguridad</h3>
                            <div class="grupo-checkbox-premium">
                                <asp:CheckBoxList ID="cblPermisosAuditoria" runat="server" RepeatLayout="UnorderedList" CssClass="checkboxlist-premium">
                                </asp:CheckBoxList>
                            </div>
                        </div>

                    </div>

                    <footer class="footer-tarjeta-acciones">
                        <asp:Button ID="btnDescartar" runat="server" Text="Descartar" CssClass="btn-secundario-gris" OnClick="btnDescartar_Click" />
                        <asp:Button ID="btnGuardarCambios" runat="server" Text="Guardar Cambios de Accesos" CssClass="btn-primario-rojo" OnClick="btnGuardarCambios_Click" />
                    </footer>

                </div>
            </section>

        </div>
    </div>
</asp:Content>
