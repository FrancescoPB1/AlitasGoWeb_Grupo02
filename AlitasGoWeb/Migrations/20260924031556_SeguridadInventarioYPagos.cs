using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlitasGoWeb.Migrations
{
    /// <inheritdoc />
    public partial class SeguridadInventarioYPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pedidos_IdEstadoPedido",
                table: "Pedidos");

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "PedidosDelivery",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoEnvio",
                table: "PedidosDelivery",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "PedidosDelivery",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "PedidosDelivery",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoAnulacion",
                table: "Pedidos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioRegistro",
                table: "Pedidos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentoCliente",
                table: "Pagos",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Monto",
                table: "Pagos",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NumeroComprobante",
                table: "Pagos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Pagos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoComprobante",
                table: "Pagos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCobro",
                table: "Pagos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "DetallesCompraInsumo",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "ComprasInsumos",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    IdAuditoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdEntidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.IdAuditoria);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    IdMovimientoInventario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInsumo = table.Column<int>(type: "int", nullable: false),
                    IdPedido = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.IdMovimientoInventario);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "IdInsumo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Pedidos_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedidos",
                        principalColumn: "IdPedido",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EstadosPedido",
                columns: new[] { "IdEstadoPedido", "Activo", "Nombre" },
                values: new object[] { 7, true, "Servido" });

            migrationBuilder.InsertData(
                table: "Insumos",
                columns: new[] { "IdInsumo", "Nombre", "StockActual", "StockMinimo", "UnidadMedida" },
                values: new object[,]
                {
                    { 5, "Salsa acevichada", 3m, 1m, "lt" },
                    { 6, "Papas", 15m, 4m, "kg" }
                });

            migrationBuilder.InsertData(
                table: "Promociones",
                columns: new[] { "IdPromocion", "Activo", "IdCanalPromocion", "IdDia", "IdTipoPromocion", "Nombre" },
                values: new object[,]
                {
                    { 1, true, 1, 2, 3, "Martes 2x1 en media docena" },
                    { 2, true, 3, 4, 2, "Docena del día (jueves)" }
                });

            migrationBuilder.InsertData(
                table: "Recetas",
                columns: new[] { "IdProductoInsumo", "Activo", "CantidadUsada", "IdInsumo", "IdProducto", "IdSabor", "UnidadMedida" },
                values: new object[,]
                {
                    { 1, true, 0.60m, 1, 1, null, "kg" },
                    { 2, true, 0.05m, 2, 1, 1, "lt" },
                    { 3, true, 0.05m, 3, 1, 2, "lt" },
                    { 5, true, 1.20m, 1, 2, null, "kg" },
                    { 6, true, 0.10m, 2, 2, 1, "lt" },
                    { 7, true, 0.10m, 3, 2, 2, "lt" },
                    { 9, true, 2.40m, 1, 3, null, "kg" },
                    { 10, true, 0.20m, 2, 3, 1, "lt" },
                    { 11, true, 0.20m, 3, 3, 2, "lt" },
                    { 14, true, 1m, 4, 9, null, "unidad" }
                });

            migrationBuilder.InsertData(
                table: "PromocionProducto",
                columns: new[] { "ProductosIdProducto", "PromocionesIdPromocion" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Recetas",
                columns: new[] { "IdProductoInsumo", "Activo", "CantidadUsada", "IdInsumo", "IdProducto", "IdSabor", "UnidadMedida" },
                values: new object[,]
                {
                    { 4, true, 0.05m, 5, 1, 3, "lt" },
                    { 8, true, 0.10m, 5, 2, 3, "lt" },
                    { 12, true, 0.20m, 5, 3, 3, "lt" },
                    { 13, true, 0.25m, 6, 6, null, "kg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_Estado_Fecha",
                table: "Pedidos",
                columns: new[] { "IdEstadoPedido", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Fecha",
                table: "Auditorias",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_Fecha",
                table: "MovimientosInventario",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdInsumo",
                table: "MovimientosInventario",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdPedido",
                table: "MovimientosInventario",
                column: "IdPedido");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_Estado_Fecha",
                table: "Pedidos");

            migrationBuilder.DeleteData(
                table: "EstadosPedido",
                keyColumn: "IdEstadoPedido",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "PromocionProducto",
                keyColumns: new[] { "ProductosIdProducto", "PromocionesIdPromocion" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "PromocionProducto",
                keyColumns: new[] { "ProductosIdProducto", "PromocionesIdPromocion" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Recetas",
                keyColumn: "IdProductoInsumo",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Insumos",
                keyColumn: "IdInsumo",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Insumos",
                keyColumn: "IdInsumo",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Promociones",
                keyColumn: "IdPromocion",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Promociones",
                keyColumn: "IdPromocion",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "CostoEnvio",
                table: "PedidosDelivery");

            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "PedidosDelivery");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "PedidosDelivery");

            migrationBuilder.DropColumn(
                name: "MotivoAnulacion",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "UsuarioRegistro",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "DocumentoCliente",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Monto",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "NumeroComprobante",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "TipoComprobante",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioCobro",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "DetallesCompraInsumo");

            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "ComprasInsumos");

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "PedidosDelivery",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdEstadoPedido",
                table: "Pedidos",
                column: "IdEstadoPedido");
        }
    }
}
