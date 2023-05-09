using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smartelectronics.Migrations
{
    public partial class ProductLoanRangeTableColumnTypeCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "InterestForVipUsers",
                table: "ProductLoanRanges",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<double>(
                name: "InterestForStandartUsers",
                table: "ProductLoanRanges",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "InterestForVipUsers",
                table: "ProductLoanRanges",
                type: "money",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestForStandartUsers",
                table: "ProductLoanRanges",
                type: "money",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
