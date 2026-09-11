using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlitasGoWeb.Migrations
{
    /// <inheritdoc />
    public partial class InicialAlitasGo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanalesAtencion",
                columns: table => new
                {
                    IdCanalAtencion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanalesAtencion", x => x.IdCanalAtencion);
                });

            migrationBuilder.CreateTable(
                name: "CanalesPromocion",
                columns: table => new
                {
                    IdCanalPromocion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanalesPromocion", x => x.IdCanalPromocion);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasProducto",
                columns: table => new
                {
                    IdCategoriaProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasProducto", x => x.IdCategoriaProducto);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DNI = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "ComprasInsumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasInsumos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dias",
                columns: table => new
                {
                    IdDia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dias", x => x.IdDia);
                });

            migrationBuilder.CreateTable(
                name: "EstadosPedido",
                columns: table => new
                {
                    IdEstadoPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosPedido", x => x.IdEstadoPedido);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    IdInsumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.IdInsumo);
                });

            migrationBuilder.CreateTable(
                name: "Sabores",
                columns: table => new
                {
                    IdSabor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sabores", x => x.IdSabor);
                });

            migrationBuilder.CreateTable(
                name: "TiposPago",
                columns: table => new
                {
                    IdTipoPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPago", x => x.IdTipoPago);
                });

            migrationBuilder.CreateTable(
                name: "TiposProducto",
                columns: table => new
                {
                    IdTipoProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposProducto", x => x.IdTipoProducto);
                });

            migrationBuilder.CreateTable(
                name: "TiposPromocion",
                columns: table => new
                {
                    IdTipoPromocion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPromocion", x => x.IdTipoPromocion);
                });

            migrationBuilder.CreateTable(
                name: "ZonasDelivery",
                columns: table => new
                {
                    IdZonaDelivery = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreZona = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostoDelivery = table.Column<decimal>(type: "Decimal(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonasDelivery", x => x.IdZonaDelivery);
                });

            migrationBuilder.CreateTable(
                name: "ZonasLocal",
                columns: table => new
                {
                    IdZonaLocal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonasLocal", x => x.IdZonaLocal);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdCanalAtencion = table.Column<int>(type: "int", nullable: false),
                    IdEstadoPedido = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_CanalesAtencion_IdCanalAtencion",
                        column: x => x.IdCanalAtencion,
                        principalTable: "CanalesAtencion",
                        principalColumn: "IdCanalAtencion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pedidos_EstadosPedido_IdEstadoPedido",
                        column: x => x.IdEstadoPedido,
                        principalTable: "EstadosPedido",
                        principalColumn: "IdEstadoPedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesCompraInsumo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    InsumoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesCompraInsumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesCompraInsumo_ComprasInsumos_CompraId",
                        column: x => x.CompraId,
                        principalTable: "ComprasInsumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesCompraInsumo_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "IdInsumo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCategoriaProducto = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdTipoProducto = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RequiereSabor = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Productos_CategoriasProducto_IdCategoriaProducto",
                        column: x => x.IdCategoriaProducto,
                        principalTable: "CategoriasProducto",
                        principalColumn: "IdCategoriaProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_TiposProducto_IdTipoProducto",
                        column: x => x.IdTipoProducto,
                        principalTable: "TiposProducto",
                        principalColumn: "IdTipoProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promociones",
                columns: table => new
                {
                    IdPromocion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdTipoPromocion = table.Column<int>(type: "int", nullable: false),
                    IdCanalPromocion = table.Column<int>(type: "int", nullable: false),
                    IdDia = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promociones", x => x.IdPromocion);
                    table.ForeignKey(
                        name: "FK_Promociones_CanalesPromocion_IdCanalPromocion",
                        column: x => x.IdCanalPromocion,
                        principalTable: "CanalesPromocion",
                        principalColumn: "IdCanalPromocion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promociones_Dias_IdDia",
                        column: x => x.IdDia,
                        principalTable: "Dias",
                        principalColumn: "IdDia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promociones_TiposPromocion_IdTipoPromocion",
                        column: x => x.IdTipoPromocion,
                        principalTable: "TiposPromocion",
                        principalColumn: "IdTipoPromocion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mesas",
                columns: table => new
                {
                    IdNroMesa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdZonaLocal = table.Column<int>(type: "int", nullable: false),
                    NumeroMesa = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesas", x => x.IdNroMesa);
                    table.ForeignKey(
                        name: "FK_Mesas_ZonasLocal_IdZonaLocal",
                        column: x => x.IdZonaLocal,
                        principalTable: "ZonasLocal",
                        principalColumn: "IdZonaLocal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "int", nullable: false),
                    IdTipoPago = table.Column<int>(type: "int", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pagos_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagos_TiposPago_IdTipoPago",
                        column: x => x.IdTipoPago,
                        principalTable: "TiposPago",
                        principalColumn: "IdTipoPago",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidosDelivery",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "int", nullable: false),
                    IdZonaDelivery = table.Column<int>(type: "int", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosDelivery", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_PedidosDelivery_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidosDelivery_ZonasDelivery_IdZonaDelivery",
                        column: x => x.IdZonaDelivery,
                        principalTable: "ZonasDelivery",
                        principalColumn: "IdZonaDelivery",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboProductos",
                columns: table => new
                {
                    IdProductoCombo = table.Column<int>(type: "int", nullable: false),
                    IdProductoComponente = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    IdSabor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboProductos", x => new { x.IdProductoCombo, x.IdProductoComponente });
                    table.ForeignKey(
                        name: "FK_ComboProductos_Productos_IdProductoCombo",
                        column: x => x.IdProductoCombo,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComboProductos_Productos_IdProductoComponente",
                        column: x => x.IdProductoComponente,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComboProductos_Sabores_IdSabor",
                        column: x => x.IdSabor,
                        principalTable: "Sabores",
                        principalColumn: "IdSabor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesPedido",
                columns: table => new
                {
                    IdDetallePedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPedido = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdSabor = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPedido", x => x.IdDetallePedido);
                    table.ForeignKey(
                        name: "FK_DetallesPedido_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesPedido_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesPedido_Sabores_IdSabor",
                        column: x => x.IdSabor,
                        principalTable: "Sabores",
                        principalColumn: "IdSabor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    IdProductoInsumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdInsumo = table.Column<int>(type: "int", nullable: false),
                    IdSabor = table.Column<int>(type: "int", nullable: true),
                    CantidadUsada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.IdProductoInsumo);
                    table.ForeignKey(
                        name: "FK_Recetas_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "IdInsumo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recetas_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recetas_Sabores_IdSabor",
                        column: x => x.IdSabor,
                        principalTable: "Sabores",
                        principalColumn: "IdSabor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromocionProducto",
                columns: table => new
                {
                    ProductosIdProducto = table.Column<int>(type: "int", nullable: false),
                    PromocionesIdPromocion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocionProducto", x => new { x.ProductosIdProducto, x.PromocionesIdPromocion });
                    table.ForeignKey(
                        name: "FK_PromocionProducto_Productos_ProductosIdProducto",
                        column: x => x.ProductosIdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromocionProducto_Promociones_PromocionesIdPromocion",
                        column: x => x.PromocionesIdPromocion,
                        principalTable: "Promociones",
                        principalColumn: "IdPromocion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidosLocal",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "int", nullable: false),
                    IdNroMesa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosLocal", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_PedidosLocal_Mesas_IdNroMesa",
                        column: x => x.IdNroMesa,
                        principalTable: "Mesas",
                        principalColumn: "IdNroMesa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidosLocal_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CanalesAtencion",
                columns: new[] { "IdCanalAtencion", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Salon" },
                    { 2, true, "Delivery" }
                });

            migrationBuilder.InsertData(
                table: "CanalesPromocion",
                columns: new[] { "IdCanalPromocion", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Salon" },
                    { 2, true, "Delivery" },
                    { 3, true, "Todos" }
                });

            migrationBuilder.InsertData(
                table: "Dias",
                columns: new[] { "IdDia", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Lunes" },
                    { 2, true, "Martes" },
                    { 3, true, "Miercoles" },
                    { 4, true, "Jueves" },
                    { 5, true, "Viernes" },
                    { 6, true, "Sabado" },
                    { 7, true, "Domingo" }
                });

            migrationBuilder.InsertData(
                table: "EstadosPedido",
                columns: new[] { "IdEstadoPedido", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Recibido" },
                    { 2, true, "EnPreparacion" },
                    { 3, true, "Listo" },
                    { 4, true, "EnReparto" },
                    { 5, true, "Entregado" },
                    { 6, true, "Anulado" }
                });

            migrationBuilder.InsertData(
                table: "Sabores",
                columns: new[] { "IdSabor", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Buffalo" },
                    { 2, true, "BBQ" },
                    { 3, true, "Acevichada" },
                    { 4, true, "Maracuya" },
                    { 5, true, "Ajo Parmesano" },
                    { 6, true, "Honey Mustard" },
                    { 7, true, "Atomica" },
                    { 8, true, "Picante" }
                });

            migrationBuilder.InsertData(
                table: "TiposPago",
                columns: new[] { "IdTipoPago", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Efectivo" },
                    { 2, true, "Yape" },
                    { 3, true, "Tarjeta" }
                });

            migrationBuilder.InsertData(
                table: "TiposProducto",
                columns: new[] { "IdTipoProducto", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Alita" },
                    { 2, true, "Bebida" },
                    { 3, true, "Guarnicion" },
                    { 4, true, "Combo" }
                });

            migrationBuilder.InsertData(
                table: "TiposPromocion",
                columns: new[] { "IdTipoPromocion", "Activo", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "NORMAL", "Precio normal" },
                    { 2, true, "DOCENA", "Promocion docena" },
                    { 3, true, "2X1", "Promocion 2x1" },
                    { 4, true, "COMBO", "Precio combo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductos_IdProductoComponente",
                table: "ComboProductos",
                column: "IdProductoComponente");

            migrationBuilder.CreateIndex(
                name: "IX_ComboProductos_IdSabor",
                table: "ComboProductos",
                column: "IdSabor");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompraInsumo_CompraId",
                table: "DetallesCompraInsumo",
                column: "CompraId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCompraInsumo_InsumoId",
                table: "DetallesCompraInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_IdPedido",
                table: "DetallesPedido",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_IdProducto",
                table: "DetallesPedido",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedido_IdSabor",
                table: "DetallesPedido",
                column: "IdSabor");

            migrationBuilder.CreateIndex(
                name: "IX_Mesas_IdZonaLocal",
                table: "Mesas",
                column: "IdZonaLocal");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdTipoPago",
                table: "Pagos",
                column: "IdTipoPago");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdCanalAtencion",
                table: "Pedidos",
                column: "IdCanalAtencion");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdCliente",
                table: "Pedidos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdEstadoPedido",
                table: "Pedidos",
                column: "IdEstadoPedido");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosDelivery_IdZonaDelivery",
                table: "PedidosDelivery",
                column: "IdZonaDelivery");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosLocal_IdNroMesa",
                table: "PedidosLocal",
                column: "IdNroMesa");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdCategoriaProducto",
                table: "Productos",
                column: "IdCategoriaProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdTipoProducto",
                table: "Productos",
                column: "IdTipoProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_IdCanalPromocion",
                table: "Promociones",
                column: "IdCanalPromocion");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_IdDia",
                table: "Promociones",
                column: "IdDia");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_IdTipoPromocion",
                table: "Promociones",
                column: "IdTipoPromocion");

            migrationBuilder.CreateIndex(
                name: "IX_PromocionProducto_PromocionesIdPromocion",
                table: "PromocionProducto",
                column: "PromocionesIdPromocion");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdInsumo",
                table: "Recetas",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdProducto",
                table: "Recetas",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdSabor",
                table: "Recetas",
                column: "IdSabor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboProductos");

            migrationBuilder.DropTable(
                name: "DetallesCompraInsumo");

            migrationBuilder.DropTable(
                name: "DetallesPedido");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "PedidosDelivery");

            migrationBuilder.DropTable(
                name: "PedidosLocal");

            migrationBuilder.DropTable(
                name: "PromocionProducto");

            migrationBuilder.DropTable(
                name: "Recetas");

            migrationBuilder.DropTable(
                name: "ComprasInsumos");

            migrationBuilder.DropTable(
                name: "TiposPago");

            migrationBuilder.DropTable(
                name: "ZonasDelivery");

            migrationBuilder.DropTable(
                name: "Mesas");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Promociones");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Sabores");

            migrationBuilder.DropTable(
                name: "ZonasLocal");

            migrationBuilder.DropTable(
                name: "CanalesAtencion");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "EstadosPedido");

            migrationBuilder.DropTable(
                name: "CanalesPromocion");

            migrationBuilder.DropTable(
                name: "Dias");

            migrationBuilder.DropTable(
                name: "TiposPromocion");

            migrationBuilder.DropTable(
                name: "CategoriasProducto");

            migrationBuilder.DropTable(
                name: "TiposProducto");
        }
    }
}
