using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlitasGoWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriaProductos",
                columns: table => new
                {
                    IdCategoriaProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProductos", x => x.IdCategoriaProducto);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    IdInsumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StockActual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.IdInsumo);
                });

            migrationBuilder.CreateTable(
                name: "TipoProductos",
                columns: table => new
                {
                    IdTipoProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProductos", x => x.IdTipoProducto);
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
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                    table.ForeignKey(
                        name: "FK_Productos_CategoriaProductos_IdCategoriaProducto",
                        column: x => x.IdCategoriaProducto,
                        principalTable: "CategoriaProductos",
                        principalColumn: "IdCategoriaProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_TipoProductos_IdTipoProducto",
                        column: x => x.IdTipoProducto,
                        principalTable: "TipoProductos",
                        principalColumn: "IdTipoProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductoInsumos",
                columns: table => new
                {
                    IdProductoInsumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    IdInsumo = table.Column<int>(type: "int", nullable: false),
                    CantidadUsada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoInsumos", x => x.IdProductoInsumo);
                    table.ForeignKey(
                        name: "FK_ProductoInsumos_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "IdInsumo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoInsumos_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductoInsumos_IdInsumo",
                table: "ProductoInsumos",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoInsumos_IdProducto",
                table: "ProductoInsumos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdCategoriaProducto",
                table: "Productos",
                column: "IdCategoriaProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_IdTipoProducto",
                table: "Productos",
                column: "IdTipoProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductoInsumos");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "CategoriaProductos");

            migrationBuilder.DropTable(
                name: "TipoProductos");
        }
    }
}
