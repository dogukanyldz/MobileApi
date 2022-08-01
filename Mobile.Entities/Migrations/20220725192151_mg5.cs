using Microsoft.EntityFrameworkCore.Migrations;

namespace Mobile.Entities.Migrations
{
    public partial class mg5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marka2",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Marka2",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
