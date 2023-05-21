using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class updatedCategoryBrands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CategoryBrands_CategoryBrandId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryBrandId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryBrandId",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryBrandId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryBrandId",
                table: "Products",
                column: "CategoryBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CategoryBrands_CategoryBrandId",
                table: "Products",
                column: "CategoryBrandId",
                principalTable: "CategoryBrands",
                principalColumn: "Id");
        }
    }
}
