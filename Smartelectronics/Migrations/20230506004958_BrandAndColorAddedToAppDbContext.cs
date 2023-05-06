using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class BrandAndColorAddedToAppDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brand_Categories_CategoryId",
                table: "Brand");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductColor_Color_ColorId",
                table: "ProductColor");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brand_BrandId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Color",
                table: "Color");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brand",
                table: "Brand");

            migrationBuilder.RenameTable(
                name: "Color",
                newName: "Colors");

            migrationBuilder.RenameTable(
                name: "Brand",
                newName: "Brands");

            migrationBuilder.RenameIndex(
                name: "IX_Brand_CategoryId",
                table: "Brands",
                newName: "IX_Brands_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colors",
                table: "Colors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brands",
                table: "Brands",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Categories_CategoryId",
                table: "Brands",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColor_Colors_ColorId",
                table: "ProductColor",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Categories_CategoryId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductColor_Colors_ColorId",
                table: "ProductColor");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Colors",
                table: "Colors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brands",
                table: "Brands");

            migrationBuilder.RenameTable(
                name: "Colors",
                newName: "Color");

            migrationBuilder.RenameTable(
                name: "Brands",
                newName: "Brand");

            migrationBuilder.RenameIndex(
                name: "IX_Brands_CategoryId",
                table: "Brand",
                newName: "IX_Brand_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Color",
                table: "Color",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brand",
                table: "Brand",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Brand_Categories_CategoryId",
                table: "Brand",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColor_Color_ColorId",
                table: "ProductColor",
                column: "ColorId",
                principalTable: "Color",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brand_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");
        }
    }
}
