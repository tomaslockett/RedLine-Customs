# Notas de Auditoría y Corrección: Flujo de Compra y Sistema de Traducciones

Este documento detalla todas las piezas modificadas en el proyecto **RedLine Customs**, cómo funciona cada una dentro de la arquitectura en capas (`UI`, `BLL`, `DAL`, `BE/Entidades`, `Servicios`) y cómo probar de punta a punta tanto el flujo de compra como las traducciones multi-idioma (**Español / Inglés**).

---

## PARTE 1: Flujo Completo de Compra y Seguridad de Contraseñas

### 1. Prevención del Doble Hasheo SHA-256 de Contraseñas
- **Problema original:** Al registrarse un cliente en `RegistroCliente.aspx.cs`, la vista aplicaba `Hashing.Sha256(pass)`, luego `BLL_Cliente.Insertar` volvía a aplicar `Hashing.Sha256(...)`, y finalmente `BLL_Usuario.Insertar` lo hasheaba una tercera vez. Como consecuencia, el cliente recién registrado nunca podía iniciar sesión con su contraseña en `LogIn.aspx`.
- **Archivos modificados:**
  - `RedLine.Web/RegistroCliente.aspx.cs`: Se envía la contraseña en texto plano a la capa de negocio (`Contraseña = pass`) para que la responsabilidad del hash quede centralizada en la BLL.
  - `RedLine.Bll/BLL_Cliente.cs`:
    - Se agregó el método auxiliar `AsegurarSha256(string texto)` y `EsHashSha256(string texto)` que verifica si la cadena ya es un hash hexadecimal de 64 caracteres antes de aplicar `Hashing.Sha256(...)`.
    - Luego de llamar a `base.Insertar(cliente)` (que encripta en AES los datos sensibles para guardarlos en SQL), se restauran en memoria los valores desencriptados (`DNI`, `Nombre`, `Apellido`, `Telefono`, `Direccion`) para que el `nuevoUsuario` creado inmediatamente después reciba datos limpios y la contraseña hasheada una única vez (`passHasheada`).
    - Se agregó `DesencriptarSeguro(string valor)` en `ObtenerClientes()` para desencriptar también `c.DNI` además de `Nombre`, `Apellido`, `Telefono` y `Direccion`, permitiendo que `BuscarPorDNI`, `Facturacion.aspx` y `HistorialVentas.aspx` muestren el DNI real y no el texto cifrado en Base64.
  - `RedLine.Bll/BLL_Usuario.cs`:
    - Se agregó `EsHashSha256(string texto)` en `Insertar`, `Login` y `LoginInconsistente` para garantizar que ninguna contraseña ya hasheada vuelva a hashearse dos veces.

### 2. `Catalogo.aspx` → `personalizarAuto.aspx`
- **Archivos modificados:** `RedLine.Web/personalizarAuto.aspx` y `RedLine.Web/personalizarAuto.aspx.cs`
- **Cómo funciona:**
  - Al entrar desde el catálogo (`personalizarAuto.aspx?id={ID}`), el backend carga el `AutoBase` y asigna el atributo `data-precio-base` en `lblPrecio` (`ClientIDMode="Static"`).
  - Un script JavaScript simple escucha el evento `change` de los 5 checkboxes (`chkAleron`, `chkKitCarroceria`, `chkLlantas`, `chkSuspension`, `chkPintura`) y actualiza en vivo el precio mostrado en pantalla sumando los adicionales seleccionados (\$500, \$1.200, \$2.500, \$3.000, \$1.800).
  - Si se ingresa sin `?id=`, ahora redirige a `Catalogo.aspx` en lugar de `Inventario.aspx` (evitando expulsar al cliente por falta de permisos de inventario).
  - Al pulsar **Confirmar configuración**, guarda `Session["Checkout_AutoId"]` y `Session["Checkout_Extras"]` y redirige a `Checkout.aspx`.

### 3. `Checkout.aspx` y `Scripts/Checkout.js`
- **Archivos modificados:** `RedLine.Web/Checkout.aspx`, `RedLine.Web/Checkout.aspx.designer.cs`, `RedLine.Web/Checkout.aspx.cs` y `RedLine.Web/Scripts/Checkout.js`
- **Cómo funciona:**
  - Se sacó el botón `btnProcesarPago` fuera del contenedor `#credit-card-panel` para que permanezca visible incluso cuando el comprador selecciona **Transferencia Bancaria** o **Financiación**.
  - Se agregó el campo oculto `hfMetodoPago` (`ClientIDMode="Static"`), sincronizado desde `Scripts/Checkout.js` cada vez que el usuario cambia de método de pago (`Tarjeta de Crédito / Débito`, `Transferencia Bancaria`, `Financiación`).
  - Se agregó `ClientIDMode="Static"` a `txtCardName`, `txtCardNumber`, `txtExpiry` y `txtCVV` para que las máscaras y validaciones JS encuentren los controles de WebForms.
  - Al confirmar el pago en `btnProcesarPago_Click`:
    1. Identifica al comprador tanto si entró por `Session["ClienteSession"]` como por `SessionManager.Instancia.Usuario` (creando o vinculando su registro espejo en `Cliente` si aún no existía).
    2. Si el método elegido es **Tarjeta de Crédito / Débito**, valida en servidor que nombre, 16 dígitos de tarjeta, vencimiento `MM/AA` y CVV estén completos.
    3. Obtiene las mejoras desde `context.Mejoras` (y si la tabla estuviera vacía, crea las mejoras en memoria con sus precios oficiales para no bloquear la venta).
    4. Construye el `AutoPersonalizado` (con o sin mejoras, gracias al ajuste en `BLL_Auto.GuardarAutoEnGarage`), descuenta 1 unidad del stock del `AutoBase` (`bllAuto.RestarStock`), y registra la `Venta` + `Factura` en base de datos mediante `bllVenta.CrearVenta(nuevaVenta, nuevaFactura)`.
    5. Guarda `Session["UltimaTransaccion"]`, `Session["UltimaVenta"]` y `Session["UltimaFactura"]`, limpia los datos temporales del carrito y redirige a `PagoExitoso.aspx`.

### 4. Persistencia de Ventas y Facturas en BLL y DAL
- **Archivos modificados:** `RedLine.Bll/BLL_Auto.cs`, `RedLine.Bll/BLL_Venta.cs`, `RedLine.Dal/Mappers/DAL_Venta.cs`, `RedLine.Dal/Mappers/DAL_Factura.cs` y `RedLine.Dal/Migrations/Configuration.cs`
- **Cómo funciona:**
  - **`BLL_Auto.cs`:** En `GuardarAutoEnGarage`, se cambió la validación `auto.Mejoras.Count == 0` por `auto.Mejoras == null` para permitir guardar en el garaje vehículos comprados en configuración estándar de fábrica (sin extras).
  - **`BLL_Venta.cs`:**
    - Se reemplazaron las referencias a las subclases eliminadas (`Aleron`, `KitCarroceria`, `Llantas`, `Suspension`, `Pintura`) por instancias de `Mejora` (lo que impedía compilar `RedLine.Bll`).
    - Se implementó `CrearVenta(Venta nuevaVenta, Factura nuevaFactura)` que vincula `Venta` y `Factura`, inserta ambas mediante `_dalVenta.Insertar(nuevaVenta)` y `_dalFactura.Insertar(nuevaFactura)`, agrega el auto al garaje en memoria del cliente y registra la operación en la bitácora de eventos.
  - **`DAL_Venta.cs`:**
    - Se corrigió `NombreTabla` de `"Ventas"` a `"Venta"` (nombre real creado por Entity Framework sin pluralización).
    - Se actualizó `SqlInsertar` con `OUTPUT INSERTED.ID` e incluyendo `MetodoPago`, `Cliente_ID`, `AutoBase_ID` y `AutoPersonalizado_ID`, asignando el `ID` autogenerado a `entidad.ID`.
    - Se agregó `HidratarRelacionesVenta` al listar/obtener ventas para poblar `Cliente`, `AutoBase`, `AutoPersonalizado` (con sus `Mejoras`) y `Factura`.
  - **`DAL_Factura.cs`:**
    - Se corrigió `NombreTabla` de `"Facturas"` a `"Factura"` y se incluyó la columna `ID` en `SqlInsertar` (relación 1 a 1 con `Venta.ID`), hidratando `factura.Venta` al leer de base de datos.
  - **`Configuration.cs`:**
    - Se agregaron en el Seed las 5 mejoras base en `context.Mejoras`, el registro espejo en `context.Cliente` para el usuario `"cliente"` (con recálculo de dígito verificador en `DAL_Cliente`), y el permiso `pFacturacion` (ID 21) dentro de la familia `Cliente` (ID 27).

### 5. `PagoExitoso.aspx` → `MiGaraje.aspx` / `Facturacion.aspx` / `DescargarFactura.ashx` → `HistorialVentas.aspx`
- **Archivos modificados:** `RedLine.Web/PagoExitoso.aspx.cs`, `RedLine.Web/Global.asax.cs`, `RedLine.Web/MiGaraje.aspx.cs`, `RedLine.Web/Facturacion.aspx.cs` y `RedLine.Web/HistorialVentas.aspx.cs`
- **Cómo funciona:**
  - **`PagoExitoso.aspx.cs`:** El botón `btnGarage` ahora redirige a `MiGaraje.aspx` en lugar de `Inventario.aspx`.
  - **`Global.asax.cs`:** Se agregó `rutaActual.Contains("facturacion")` en `EsPaginaPermitidaParaCliente` para que un cliente autenticado pueda abrir `Facturacion.aspx` sin ser redirigido al login.
  - **`MiGaraje.aspx.cs`:**
    - `ObtenerClienteActual()` ahora busca al cliente por Email en `BLL_Cliente` si el usuario inició sesión por `SessionManager`, sincronizando `Session["ClienteSession"]`.
    - `MapearDatosVentaEnAuto()` asigna siempre `v.AutoPersonalizado.TotalPagado = v.Total` para que el monto abonado en la tarjeta del garaje coincida exactamente con el total facturado con IVA (21%).
  - **`Facturacion.aspx.cs`:**
    - Se agregó filtrado por seguridad en `ResolverFacturaActual()` y `BuscarVentaDeFactura()` mediante `PerteneceAlClienteActual()` y `EsUsuarioConAccesoGlobalFacturas()`. Así, un cliente común solo puede ver y descargar sus propias facturas, mientras que un `WebMaster` o `Administrador` puede inspeccionar cualquier factura.
  - **`HistorialVentas.aspx.cs`:**
    - Se eliminó el `using RedLine.BLL;` inválido (con mayúsculas) que rompía la compilación de `RedLine.Web`.
    - Se descomentó e implementó `CargarGrillaEIndicadores()` y `AsegurarDatosParaGrilla(Venta v)` usando bucles `foreach` e `if/else` simples para calcular el total recaudado, cantidad de autos vendidos, ticket promedio y poblar la grilla `gvVentas`.

---

## PARTE 2: Sistema de Traducciones (Español / Inglés)

### 1. Motor de Traducción en `BasePage.cs`
- **Archivo modificado:** `RedLine.Web/BasePage.cs`
- **Problemas corregidos:**
  1. Antes, `GridView` y `DropDownList` estaban dentro del bloque `if (EsTraduccionValida(traduccion))`, el cual buscaba una etiqueta con el `ID` de la grilla (por ejemplo `gvUsuarios`). Como las columnas tienen sus propias claves (`col_...` o `gv_col_...`) y la grilla en sí no tiene etiqueta, nunca entraba al `switch` y ningún encabezado de `GridView` se traducía.
  2. Ahora `GridView` y `DropDownList` se procesan antes de esa guarda:
     - `TraducirColumnasGridView(GridView gv)` traduce cada columna por su `HeaderText` original (guardado en `AccessibleHeaderText` para soportar múltiples postbacks) y actualiza también `gv.HeaderRow.Cells[i].Text` si la grilla ya hizo `DataBind()` en `Page_Load`.
     - `TraducirItemsDropDownList(DropDownList ddl)` traduce el ítem por defecto (`Value == ""` o `"0"`) con la clave `{ddl.ID}` y cada opción hija buscando `{ddl.ID}_{i}` o `{ddl.ID}_{item.Value}`.
  3. Se agregó soporte para controles `HyperLink` (`case HyperLink hlk:`) preservando íconos HTML internos si los hubiera.

### 2. Corrección de Colisiones de `Clave` entre Páginas
Como `Etiqueta.Clave` tiene índice único en base de datos, se renombraron los 5 controles que colisionaban entre distintas páginas (en `.aspx`, `.designer.cs` y `Configuration.cs`):
1. `BackupRestore.aspx`: `lblTitulo` → `lblTituloBackup` (evita pisar `lblTitulo` de `LogIn.aspx`).
2. `Logout.aspx`: `btnConfirmar` → `btnConfirmarLogout` (evita pisar `btnConfirmar` de `personalizarAuto.aspx`).
3. `CrearAuto.aspx`: `lblPotencia` → `lblPotenciaCrear` y `lblAceleracion` → `lblAceleracionCrear` (evita pisar las etiquetas de `Catalogo.aspx`), y se agregó la etiqueta `DropDownListMarca`.
4. `GestionEventos.aspx`: `lblFiltroFechaDesde` → `lblFiltroFechaEventos` (evita pisar `lblFiltroFechaDesde` de `HistorialVentas.aspx`), y se agregaron las etiquetas para `btnExportar`, `ddlFiltroCri_0..3`, `msg_error_exportar_permisos` y `msg_error_exportar_bitacora`.

### 3. Limpieza del Seed y `GestionIdiomas.aspx`
- **Archivos modificados:** `RedLine.Web/GestionIdiomas.aspx` y `RedLine.Dal/Migrations/Configuration.cs`
- Se corrigieron los `Value` del desplegable `ddlFiltroPagina` en `GestionIdiomas.aspx` y sus claves en `Configuration.cs` para que coincidan exactamente con el campo `Etiqueta.Pagina` (`LogIn.aspx`, `Logout.aspx`, `RegistroCliente.aspx`, `personalizarAuto.aspx`, `Gestion_Perfiles_Permisos.aspx`, `RecuperacionDV.aspx`).
- Se reemplazaron los `HeaderText` fijos de `gvTraducciones` por sus claves (`col_PantallaModulo`, `col_ClaveEtiqueta`, `col_DescripcionUso`, `col_TipoElemento`, `col_TraduccionTexto`) y los textos del badge de tipo por `Traducir("texto_tipo_alerta")` y `Traducir("texto_tipo_control")`.
- Se eliminaron los 2 bloques duplicados de `context.Traducciones.AddOrUpdate` en español dentro de `Configuration.cs` (`Catalogo.aspx` y `GestionEventos.aspx`).

---

## CÓMO PROBAR LOS CAMBIOS PASO A PASO

### Paso 1: Actualizar la Base de Datos (Migraciones / Seed)
1. Abrir la solución en **Visual Studio**.
2. Abrir la **Consola del Administrador de Paquetes NuGet** (*Package Manager Console*), seleccionar el proyecto predeterminado **`RedLine.Dal`** y ejecutar:
   ```powershell
   Update-Database -Verbose
   ```
   *(Esto ejecutará el método `Seed` de `Configuration.cs`, insertando las 5 mejoras base, el cliente espejo `"cliente"`, el permiso `Facturacion` para la familia `Cliente`, todas las etiquetas y traducciones corregidas en Español e Inglés, y recalculará los dígitos verificadores).*

### Paso 2: Probar el Registro y Login sin Doble Hash
1. Iniciar la aplicación y entrar a `RegistroCliente.aspx`.
2. Registrar un cliente nuevo con un correo y contraseña conocidos (por ejemplo `1234`).
3. Ir a `LogIn.aspx` e iniciar sesión con ese correo y contraseña `1234`, o bien con el usuario de prueba `cliente` / `cliente`.
4. Verificar que ingresa directamente a `Catalogo.aspx` sin rebotes ni errores de credenciales.

### Paso 3: Probar el Flujo Completo de Compra
1. En `Catalogo.aspx`, hacer clic en **Personalizar** sobre un vehículo con stock disponible.
2. En `personalizarAuto.aspx`, marcar y desmarcar distintas mejoras (Alerón, Kit de carrocería, Llantas, etc.) y verificar que el precio superior se actualiza en vivo. Hacer clic en **Confirmar configuración** (probar también sin marcar mejoras).
3. En `Checkout.aspx`:
   - Probar cambiar entre **Tarjeta de Crédito / Débito**, **Transferencia Bancaria** y **Financiación** y verificar que el botón **Confirmar y Pagar Total** siempre está visible.
   - Completar los datos o elegir Transferencia/Financiación y hacer clic en **Confirmar y Pagar Total**.
4. En `PagoExitoso.aspx`, verificar que se muestra el código de transacción (`TRX-...`) y hacer clic en **Ir a mi Garage Virtual**.
5. En `MiGaraje.aspx`, verificar que aparece el vehículo comprado con su número de factura, número de venta, método de pago, mejoras instaladas y monto total abonado (con IVA 21%).
6. Hacer clic en **Ver Factura** (abre `Facturacion.aspx?id=...`) y en **Descargar Factura (PDF)** (descarga el PDF oficial desde `DescargarFactura.ashx`).
7. Cerrar sesión, ingresar como `webmaster` (`webmaster` / `webmaster123`) o `admin` (`admin` / `admin`), ir a **Taller y Ventas → Historial de Ventas** (`HistorialVentas.aspx`) y verificar que los indicadores superiores (Total Recaudado, Autos Vendidos, Ticket Promedio) y la grilla muestran la venta recién realizada.

### Paso 4: Probar las Traducciones (Español / Inglés)
1. En la barra superior (`Site.Master`), cambiar el selector de idioma de **Español** a **Inglés (English)**.
2. Navegar por las páginas (`Catalogo.aspx`, `personalizarAuto.aspx`, `Checkout.aspx`, `PagoExitoso.aspx`, `MiGaraje.aspx`, `Facturacion.aspx`, `HistorialVentas.aspx`, `Inventario.aspx`, `CrearAuto.aspx`, `GestionUsuarios.aspx`, `GestionClientes.aspx`, `GestionEventos.aspx`, `GestionIdiomas.aspx`, `BackupRestore.aspx`, `Logout.aspx`) y verificar que todos los títulos, botones, encabezados de `GridView` y opciones de `DropDownList` se muestran traducidos sin pisarse entre páginas.
