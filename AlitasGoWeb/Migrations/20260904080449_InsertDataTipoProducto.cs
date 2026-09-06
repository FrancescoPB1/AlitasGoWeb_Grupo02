using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlitasGoWeb.Migrations
{
    /// <inheritdoc />
    public partial class InsertDataTipoProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TipoProductos",
                columns: new[] { "IdTipoProducto", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 3, true, "Simple" },
                    { 4, true, "Combo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipoProductos",
                keyColumn: "IdTipoProducto",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TipoProductos",
                keyColumn: "IdTipoProducto",
                keyValue: 2);
        }
    }
}
