using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class AddedColorsTableUpdatedLoans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanTerms_LoanRanges_LoanRangeId",
                table: "LoanTerms");

            migrationBuilder.DropIndex(
                name: "IX_LoanTerms_LoanRangeId",
                table: "LoanTerms");

            migrationBuilder.DropColumn(
                name: "LoanRangeId",
                table: "LoanTerms");

            migrationBuilder.CreateTable(
                name: "Color",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Color", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanTermLoanRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanTermId = table.Column<int>(type: "int", nullable: true),
                    LoanRangeId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTermLoanRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanTermLoanRanges_LoanRanges_LoanRangeId",
                        column: x => x.LoanRangeId,
                        principalTable: "LoanRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanTermLoanRanges_LoanTerms_LoanTermId",
                        column: x => x.LoanTermId,
                        principalTable: "LoanTerms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductColor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ColorId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColor_Color_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Color",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductColor_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanTermLoanRanges_LoanRangeId",
                table: "LoanTermLoanRanges",
                column: "LoanRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanTermLoanRanges_LoanTermId",
                table: "LoanTermLoanRanges",
                column: "LoanTermId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColor_ColorId",
                table: "ProductColor",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColor_ProductId",
                table: "ProductColor",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanTermLoanRanges");

            migrationBuilder.DropTable(
                name: "ProductColor");

            migrationBuilder.DropTable(
                name: "Color");

            migrationBuilder.AddColumn<int>(
                name: "LoanRangeId",
                table: "LoanTerms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanTerms_LoanRangeId",
                table: "LoanTerms",
                column: "LoanRangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanTerms_LoanRanges_LoanRangeId",
                table: "LoanTerms",
                column: "LoanRangeId",
                principalTable: "LoanRanges",
                principalColumn: "Id");
        }
    }
}
