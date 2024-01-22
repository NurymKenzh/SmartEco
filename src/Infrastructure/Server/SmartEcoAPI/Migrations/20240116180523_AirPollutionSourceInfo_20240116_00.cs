using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutionSourceInfo_20240116_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Coordinate3857",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coordinate3857",
                schema: "asm",
                table: "AirPollutionSourceInfo");
        }
    }
}
