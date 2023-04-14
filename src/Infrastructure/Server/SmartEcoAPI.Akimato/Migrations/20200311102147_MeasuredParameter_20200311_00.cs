using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredParameter_20200311_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MPC",
                table: "MeasuredParameter",
                newName: "MPCDailyAverage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MPCDailyAverage",
                table: "MeasuredParameter",
                newName: "MPC");
        }
    }
}
