using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class MonitoringPost_20200110_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KazhydrometID",
                table: "MonitoringPost",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KazhydrometID",
                table: "MonitoringPost");
        }
    }
}
