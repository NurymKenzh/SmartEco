using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutionSourceInfo_20231108_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "asm",
                table: "AirPollutionSourceInfo");
        }
    }
}
