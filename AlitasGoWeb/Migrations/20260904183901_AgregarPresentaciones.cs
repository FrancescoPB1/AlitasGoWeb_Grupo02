using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlitasGoWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPresentaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PresentacionProductos",
                columns: table => new
                {
                    IdPresentacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    NombrePresentacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CantidadUnidad = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionProductos", x => x.IdPresentacion);
                    table.ForeignKey(
                        name: "FK_PresentacionProductos_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PresentacionProductos_IdProducto",
                table: "PresentacionProductos",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PresentacionProductos");
        }
    }
}
