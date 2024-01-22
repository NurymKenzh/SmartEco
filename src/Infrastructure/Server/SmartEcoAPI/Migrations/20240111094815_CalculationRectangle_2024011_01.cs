using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class CalculationRectangle_2024011_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicateZ",
                schema: "asmUprza",
                table: "CalculationRectangle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicateZ",
                schema: "asmUprza",
                table: "CalculationRectangle",
                nullable: false,
                defaultValue: 0);
        }
    }
}
