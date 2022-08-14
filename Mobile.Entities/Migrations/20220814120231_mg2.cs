using Microsoft.EntityFrameworkCore.Migrations;

namespace Mobile.Entities.Migrations
{
    public partial class mg2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SetProducts_SetProductsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SetProductsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SetProductsId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "SetProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SetProducts_ProductId",
                table: "SetProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetProducts_Products_ProductId",
                table: "SetProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetProducts_Products_ProductId",
                table: "SetProducts");

            migrationBuilder.DropIndex(
                name: "IX_SetProducts_ProductId",
                table: "SetProducts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "SetProducts");

            migrationBuilder.AddColumn<int>(
                name: "SetProductsId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SetProductsId",
                table: "Products",
                column: "SetProductsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SetProducts_SetProductsId",
                table: "Products",
                column: "SetProductsId",
                principalTable: "SetProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
