using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class UpdatedProductCategorySpecifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySpecifications_Categories_CategoryId",
                table: "CategorySpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySpecifications_Specifications_SpecificationId",
                table: "CategorySpecifications");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductCategorySpecifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<int>(
                name: "SpecificationId",
                table: "CategorySpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategorySpecifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySpecifications_Categories_CategoryId",
                table: "CategorySpecifications",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySpecifications_Specifications_SpecificationId",
                table: "CategorySpecifications",
                column: "SpecificationId",
                principalTable: "Specifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySpecifications_Categories_CategoryId",
                table: "CategorySpecifications");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySpecifications_Specifications_SpecificationId",
                table: "CategorySpecifications");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ProductCategorySpecifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SpecificationId",
                table: "CategorySpecifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategorySpecifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySpecifications_Categories_CategoryId",
                table: "CategorySpecifications",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySpecifications_Specifications_SpecificationId",
                table: "CategorySpecifications",
                column: "SpecificationId",
                principalTable: "Specifications",
                principalColumn: "Id");
        }
    }
}
