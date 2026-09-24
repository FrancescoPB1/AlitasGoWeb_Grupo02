# AlitasGo Web · Grupo 02

Sistema de pedidos, cocina y reparto para **AlitasGo** (caso de estudio del curso *Diseño y Arquitectura de Software*, UPN).
ASP.NET Core MVC 8 + Entity Framework Core 8 + SQL Server, monolito en capas.

## 1. Puesta en marcha (Visual Studio 2022)

1. **Cadena de conexión.** En `AlitasGoWeb/appsettings.json` ajusta `Server=` a tu instancia
   (por defecto `.\SQLEXPRESS`, base `AlitasGoDB`).
2. **Contraseña inicial de los usuarios** (no va en Git: se guarda en *User Secrets*).
   En la carpeta `AlitasGoWeb` ejecuta:
   ```
   dotnet user-secrets set "Seed:PasswordInicial" "<contraseña inicial>"
   ```
   Debe tener mínimo 10 caracteres, un número, una mayúscula y un símbolo (ej. de formato: `Alitas#2026x`).
   Cada persona debe cambiarla luego desde *Cambiar contraseña*.
3. **Base de datos.** En la *Consola del Administrador de paquetes* (proyecto AlitasGoWeb):
   ```
   Update-Database
   ```
   Aplica las 3 migraciones: `InicialAlitasGo`, `InsercionValores` y `SeguridadInventarioYPagos`.
4. **Ejecutar** con el perfil `https`. Al arrancar se crean los roles y estos usuarios:

   | Usuario | Rol | Pantalla inicial |
   |---|---|---|
   | rosa@alitasgo.pe | Administrador | Pedidos (y menú Administración) |
   | kevin@alitasgo.pe | Cajero | Salón |
   | italo@alitasgo.pe | Cocinero | Cocina |
   | brayan@alitasgo.pe | Repartidor | Reparto |

5. **Segundo factor de Rosa.** Con solo su contraseña, Rosa entra a todo, incluida la Administración
   (política `SoloAdmin` = rol Administrador, igual que el material). **Anular pedidos** exige además 2FA
   (política `AnularPedido`): menú de usuario → *Seguridad (2FA)* → configurar la app autenticadora
   (Google/Microsoft Authenticator, ingresando la clave que muestra la página) → **cerrar sesión y volver
   a entrar** escribiendo el código.

## 2. Pruebas

```
dotnet test
```
`AlitasGoWeb.Tests` (xUnit) incluye:
- **Unitarias:** estrategias de precio y selección de estrategia, máquina de estados, día de promoción,
  reglas del Pedido/Insumo, IGV incluido, Singleton.
- **Integración** (SQLite en memoria con el mismo modelo y semillas): registrar salón/delivery, promociones
  por día y canal, combos, stock insuficiente sin guardar nada, edición, anulación con/sin reposición,
  cobro con boleta/factura, compras, bitácora y reportes.
- **Seguridad:** matriz rol × política, 2FA del Administrador, política de cada controlador,
  token antifalsificación en todo POST.
- **Tiempo real:** a qué rol le llega cada aviso y que cada operación del pedido emita el suyo.

## 3. Arquitectura

```
Presentación   Controllers/, Views/, ViewModels/, Areas/Identity (login y 2FA en español)
Servicios      Services/ (PedidoService, PrecioService + Services/Precios (Strategy), InventarioService,
               PromocionService, CatalogoService, ReporteService, AuditoriaService, UsuarioService,
               CalculadoraIgv, ConfiguracionSistema (Singleton))
Dominio        Models/ (entidades, MaquinaEstadosPedido, constantes de estados y canales)
Repositorios   Data/Repositories/ (+ UnidadDeTrabajo con transacción) → Data/AlitasGoDbContext
Seguridad      Seguridad/ (roles, políticas, semilla de usuarios, mensajes de Identity en español)
```
Regla: ningún controlador usa el `DbContext`; todo pasa por servicios y repositorios.

## 4. Tiempo real (SignalR)

SignalR viene incluido en ASP.NET Core 8; el cliente JS está en `wwwroot/lib/microsoft/signalr` (v8.0.0).

- `Hubs/PedidosHub` (`/hubs/pedidos`, requiere sesión) agrupa cada conexión por rol: admin, caja, cocina, reparto.
- `PedidoService` avisa por `INotificadorPedidos` **después** de confirmar cada operación
  (registrar, editar, cambio de estado, anular, cobrar). `Hubs/NotificadorPedidosSignalR` decide a quién llega.
- Qué se actualiza solo: tablero de **Cocina** (con aviso por voz, incluso de pedidos anulados o modificados),
  mesas del **Salón**, **Reparto**, lista de **Pedidos** y el detalle abierto de un pedido.
- Avisos emergentes: "Pedido #12 listo para servir · Mesa 3" (caja), "Pedido #13 listo para despachar"
  (reparto), anulaciones y entregas. El indicador **● En vivo** de la barra muestra el estado de la conexión;
  si se corta, reconecta solo y la cocina además consulta cada 30 s como respaldo.

## 5. Estados del pedido

`Recibido → En preparación → Listo → (En reparto | Servido) → Entregado`, y `Anulado` desde cualquier
estado previo a la entrega. Solo se edita en *Recibido*; al anular en *Recibido/En preparación* los insumos
vuelven al stock; *Entregado* exige registrar el cobro (boleta o factura).
