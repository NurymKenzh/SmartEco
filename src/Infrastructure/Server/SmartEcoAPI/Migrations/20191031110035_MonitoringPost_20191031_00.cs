using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class MonitoringPost_20191031_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxMeasuredValue",
                table: "MonitoringPostMeasuredParameters",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinMeasuredValue",
                table: "MonitoringPostMeasuredParameters",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxMeasuredValue",
                table: "MonitoringPostMeasuredParameters");

            migrationBuilder.DropColumn(
                name: "MinMeasuredValue",
                table: "MonitoringPostMeasuredParameters");
        }
    }
}
