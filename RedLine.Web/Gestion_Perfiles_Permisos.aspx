<%@ Page Title="Gestión de Perfiles" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Gestion_Perfiles_Permisos.aspx.cs" Inherits="RedLine.Web.Gestion_Perfiles_Permisos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/GestionPermisos.css" rel="stylesheet" type="text/css" />

    <div class="contenedor-permisos animate-fade-in">
        
        <header class="encabezado-seccion">
            <div class="header-navegacion">
                <asp:LinkButton ID="btnVolverDashboard" runat="server" CssClass="btn-volver" PostBackUrl="~/AdminDashboard.aspx">
                    <span>←</span> <asp:Label ID="lblVolverDashboard" runat="server" Text="Volver al Dashboard" />
                </asp:LinkButton>
            </div>
            <h1 class="titulo-principal"><asp:Label ID="lblTituloSeguridad" runat="server" Text="Seguridad: Perfiles y Permisos" /></h1>
            <p class="subtitulo-tecnico"><asp:Label ID="lblSubtituloSeguridad" runat="server" Text="Configuración de roles jerárquicos y asignación de privilegios inmutables del sistema (Patrón Composite)." /></p>
        </header>

        <div class="split-panel">
            
            <section class="columna-perfiles">
                <div class="tarjeta-glassmorphic">
                    <h2 class="titulo-tarjeta"><asp:Label ID="lblPerfilesSistema" runat="server" Text="Perfiles del Sistema" /></h2>
                    <p class="descripcion-tarjeta"><asp:Label ID="lblDescPerfiles" runat="server" Text="Creá o seleccioná un perfil (Familia Composite) para administrar sus accesos." /></p>
                    
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
                        <asp:Label ID="lblTextoPermisosAsignados" runat="server" Text="Permisos asignados a: " />
                        <span class="rol-seleccionado-badge"><asp:Label ID="lblPerfilSeleccionado" runat="server" Text="Seleccione un perfil..."></asp:Label></span>
                    </h2>
                    <p class="descripcion-tarjeta info-alerta"><asp:Label ID="lblAdvertenciaInmutable" runat="server" Text="Las patentes listadas son inmutables a nivel código. Solo podés activar o desactivar su relación con la familia seleccionada." /></p>
                    
                    <div class="matriz-scroll-contenedor">
                        
                        <div class="bloque-modulo">
                            <h3 class="titulo-modulo-interno"><asp:Label ID="lblModCatalogoInv" runat="server" Text="Módulo Catálogo e Inventario" /></h3>
                            <div class="grupo-checkbox-premium">
                                <asp:CheckBoxList ID="cblPermisosCatalogo" runat="server" RepeatLayout="UnorderedList" CssClass="checkboxlist-premium">
                                </asp:CheckBoxList>
                            </div>
                        </div>

                        <div class="bloque-modulo">
                            <h3 class="titulo-modulo-interno"><asp:Label ID="lblModComercialVentas" runat="server" Text="Módulo Comercial y Ventas" /></h3>
                            <div class="grupo-checkbox-premium">
                                <asp:CheckBoxList ID="cblPermisosVentas" runat="server" RepeatLayout="UnorderedList" CssClass="checkboxlist-premium">
                                </asp:CheckBoxList>
                            </div>
                        </div>

                        <div class="bloque-modulo">
                            <h3 class="titulo-modulo-interno"><asp:Label ID="lblModAuditoriaSeg" runat="server" Text="Módulo Auditoría y Seguridad" /></h3>
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