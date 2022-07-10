using Microsoft.EntityFrameworkCore.Migrations;

namespace Mobile.Entities.Migrations
{
    public partial class mg4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Durums",
                table: "Products",
                newName: "Durum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Durum",
                table: "Products",
                newName: "Durums");
        }
    }
}
