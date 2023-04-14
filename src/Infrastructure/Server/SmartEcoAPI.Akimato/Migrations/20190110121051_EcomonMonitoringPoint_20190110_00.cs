using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class EcomonMonitoringPoint_20190110_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EastLongitude",
                table: "EcomonMonitoringPoint",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NorthLatitude",
                table: "EcomonMonitoringPoint",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EastLongitude",
                table: "EcomonMonitoringPoint");

            migrationBuilder.DropColumn(
                name: "NorthLatitude",
                table: "EcomonMonitoringPoint");
        }
    }
}
