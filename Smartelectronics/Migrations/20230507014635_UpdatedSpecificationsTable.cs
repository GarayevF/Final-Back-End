using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class UpdatedSpecificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecificationGroupId",
                table: "Specifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_SpecificationGroupId",
                table: "Specifications",
                column: "SpecificationGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Specifications_SpecificationGroups_SpecificationGroupId",
                table: "Specifications",
                column: "SpecificationGroupId",
                principalTable: "SpecificationGroups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specifications_SpecificationGroups_SpecificationGroupId",
                table: "Specifications");

            migrationBuilder.DropIndex(
                name: "IX_Specifications_SpecificationGroupId",
                table: "Specifications");

            migrationBuilder.DropColumn(
                name: "SpecificationGroupId",
                table: "Specifications");
        }
    }
}
