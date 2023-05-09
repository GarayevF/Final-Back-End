using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class AddedInterestLogic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductIFLoanRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    LoanRangeId = table.Column<int>(type: "int", nullable: false),
                    InitialPayment = table.Column<decimal>(type: "money", nullable: true),
                    MonthlyPayment = table.Column<decimal>(type: "money", nullable: true),
                    TotalPayment = table.Column<decimal>(type: "money", nullable: true),
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
                    table.PrimaryKey("PK_ProductIFLoanRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductIFLoanRanges_LoanRanges_LoanRangeId",
                        column: x => x.LoanRangeId,
                        principalTable: "LoanRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductIFLoanRanges_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductLoanRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    LoanRangeId = table.Column<int>(type: "int", nullable: false),
                    InterestForStandartUsers = table.Column<decimal>(type: "money", nullable: false),
                    InterestForVipUsers = table.Column<decimal>(type: "money", nullable: false),
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
                    table.PrimaryKey("PK_ProductLoanRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLoanRanges_LoanRanges_LoanRangeId",
                        column: x => x.LoanRangeId,
                        principalTable: "LoanRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductLoanRanges_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductIFLoanRanges_LoanRangeId",
                table: "ProductIFLoanRanges",
                column: "LoanRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductIFLoanRanges_ProductId",
                table: "ProductIFLoanRanges",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoanRanges_LoanRangeId",
                table: "ProductLoanRanges",
                column: "LoanRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoanRanges_ProductId",
                table: "ProductLoanRanges",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductIFLoanRanges");

            migrationBuilder.DropTable(
                name: "ProductLoanRanges");
        }
    }
}
