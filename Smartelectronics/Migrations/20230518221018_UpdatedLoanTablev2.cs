using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class UpdatedLoanTablev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanTerms_LoanCompanies_LoanCompanyId",
                table: "LoanTerms");

            migrationBuilder.DropColumn(
                name: "ExTax",
                table: "Baskets");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPayment",
                table: "ProductIFLoanRanges",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyPayment",
                table: "ProductIFLoanRanges",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialPayment",
                table: "ProductIFLoanRanges",
                type: "money",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LoanCompanyId",
                table: "LoanTerms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanTerms_LoanCompanies_LoanCompanyId",
                table: "LoanTerms",
                column: "LoanCompanyId",
                principalTable: "LoanCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanTerms_LoanCompanies_LoanCompanyId",
                table: "LoanTerms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPayment",
                table: "ProductIFLoanRanges",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<decimal>(
                name: "MonthlyPayment",
                table: "ProductIFLoanRanges",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<decimal>(
                name: "InitialPayment",
                table: "ProductIFLoanRanges",
                type: "money",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "LoanCompanyId",
                table: "LoanTerms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "ExTax",
                table: "Baskets",
                type: "float",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanTerms_LoanCompanies_LoanCompanyId",
                table: "LoanTerms",
                column: "LoanCompanyId",
                principalTable: "LoanCompanies",
                principalColumn: "Id");
        }
    }
}
