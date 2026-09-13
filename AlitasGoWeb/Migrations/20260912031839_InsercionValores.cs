using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlitasGoWeb.Migrations
{
    /// <inheritdoc />
    public partial class InsercionValores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposPromocion",
                keyColumn: "IdTipoPromocion",
                keyValue: 4);

            migrationBuilder.InsertData(
                table: "CategoriasProducto",
                columns: new[] { "IdCategoriaProducto", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Alitas" },
                    { 2, true, "Boneless" },
                    { 3, true, "Guarniciones" },
                    { 4, true, "Bebidas" }
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "IdCliente", "Activo", "ApellidoMaterno", "ApellidoPaterno", "DNI", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Gomez", "Perez", "12345678", "Juan" },
                    { 2, true, "Diaz", "Lopez", "87654321", "Maria" },
                    { 3, true, "Torres", "Ramirez", "11223344", "Carlos" }
                });

            migrationBuilder.InsertData(
                table: "Insumos",
                columns: new[] { "IdInsumo", "Nombre", "StockActual", "StockMinimo", "UnidadMedida" },
                values: new object[,]
                {
                    { 1, "Alitas crudas", 20m, 5m, "kg" },
                    { 2, "Salsa buffalo", 5m, 2m, "lt" },
                    { 3, "Salsa BBQ", 4m, 2m, "lt" },
                    { 4, "Gaseosa personal", 50m, 12m, "unidad" }
                });

            migrationBuilder.UpdateData(
                table: "TiposProducto",
                keyColumn: "IdTipoProducto",
                keyValue: 1,
                column: "Nombre",
                value: "Plato");

            migrationBuilder.InsertData(
                table: "ZonasDelivery",
                columns: new[] { "IdZonaDelivery", "Activo", "CostoDelivery", "NombreZona" },
                values: new object[,]
                {
                    { 1, true, 3.00m, "Cerca" },
                    { 2, true, 5.00m, "Media" },
                    { 3, true, 8.00m, "Bordes" }
                });

            migrationBuilder.InsertData(
                table: "ZonasLocal",
                columns: new[] { "IdZonaLocal", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Interior" },
                    { 2, true, "Vereda" }
                });

            migrationBuilder.InsertData(
                table: "Mesas",
                columns: new[] { "IdNroMesa", "Activo", "IdZonaLocal", "NumeroMesa" },
                values: new object[,]
                {
                    { 1, true, 1, 1 },
                    { 2, true, 1, 2 },
                    { 3, true, 1, 3 },
                    { 4, true, 1, 4 },
                    { 5, true, 1, 5 },
                    { 6, true, 2, 6 },
                    { 7, true, 2, 7 },
                    { 8, true, 2, 8 },
                    { 9, true, 2, 9 },
                    { 10, true, 2, 10 }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "IdProducto", "Activo", "Descripcion", "IdCategoriaProducto", "IdTipoProducto", "Nombre", "Precio", "RequiereSabor" },
                values: new object[,]
                {
                    { 1, true, "Media docena de alitas", 1, 1, "Media Docena Alitas", 18.00m, true },
                    { 2, true, "Docena de alitas", 1, 1, "Docena Alitas", 30.00m, true },
                    { 3, true, "Dos docenas de alitas", 1, 1, "2 Docenas Alitas", 55.00m, true },
                    { 4, true, "Media docena boneless", 2, 1, "Media Docena Boneless", 20.00m, true },
                    { 5, true, "Docena boneless", 2, 1, "Docena Boneless", 35.00m, true },
                    { 6, true, "Porcion de papas", 3, 3, "Papas Fritas", 8.00m, false },
                    { 7, true, "Porcion de yucas", 3, 3, "Yucas Fritas", 9.00m, false },
                    { 8, true, "Ensalada fresca", 3, 3, "Ensalada", 7.00m, false },
                    { 9, true, "Gaseosa 500ml", 4, 2, "Gaseosa Personal", 5.00m, false },
                    { 10, true, "Gaseosa 1.5L", 4, 2, "Gaseosa 1.5L", 12.00m, false },
                    { 11, true, "Vaso de chicha", 4, 2, "Chicha Vaso", 4.00m, false },
                    { 12, true, "Jarra de chicha", 4, 2, "Chicha Jarra 1L", 10.00m, false },
                    { 13, true, "2 docenas + 2 gaseosas personales", 1, 4, "Combo Duo Alitas", 65.00m, false }
                });

            migrationBuilder.InsertData(
                table: "ComboProductos",
                columns: new[] { "IdProductoCombo", "IdProductoComponente", "Cantidad", "IdSabor" },
                values: new object[,]
                {
                    { 13, 2, 2, 1 },
                    { 13, 9, 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "IdCliente",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "IdCliente",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "IdCliente",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ComboProductos",
                keyColumns: new[] { "IdProductoCombo", "IdProductoComponente" },
                keyValues: new object[] { 13, 2 });

            migrationBuilder.DeleteData(
                table: "ComboProductos",
                keyColumns: new[] { "IdProductoCombo", "IdProductoComponente" },
                keyValues: new object[] { 13, 9 });

            migrationBuilder.DeleteData(
                table: "Insumos",
                keyColumn: "IdInsumo",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Insumos",
                keyColumn: "IdInsumo",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Insumos",
                keyColumn: "IdInsumo",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Insumos",
                keyColumn: "IdInsumo",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Mesas",
                keyColumn: "IdNroMesa",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ZonasDelivery",
                keyColumn: "IdZonaDelivery",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ZonasDelivery",
                keyColumn: "IdZonaDelivery",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ZonasDelivery",
                keyColumn: "IdZonaDelivery",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CategoriasProducto",
                keyColumn: "IdCategoriaProducto",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategoriasProducto",
                keyColumn: "IdCategoriaProducto",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "IdProducto",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ZonasLocal",
                keyColumn: "IdZonaLocal",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ZonasLocal",
                keyColumn: "IdZonaLocal",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CategoriasProducto",
                keyColumn: "IdCategoriaProducto",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CategoriasProducto",
                keyColumn: "IdCategoriaProducto",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "TiposProducto",
                keyColumn: "IdTipoProducto",
                keyValue: 1,
                column: "Nombre",
                value: "Alita");

            migrationBuilder.InsertData(
                table: "TiposPromocion",
                columns: new[] { "IdTipoPromocion", "Activo", "Codigo", "Nombre" },
                values: new object[] { 4, true, "COMBO", "Precio combo" });
        }
    }
}
