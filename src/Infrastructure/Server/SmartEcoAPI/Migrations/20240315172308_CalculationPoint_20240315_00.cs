using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class CalculationPoint_20240315_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Abscissa3857",
                schema: "asmUprza",
                table: "CalculationPoint",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Ordinate3857",
                schema: "asmUprza",
                table: "CalculationPoint",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abscissa3857",
                schema: "asmUprza",
                table: "CalculationPoint");

            migrationBuilder.DropColumn(
                name: "Ordinate3857",
                schema: "asmUprza",
                table: "CalculationPoint");
        }
    }
}
