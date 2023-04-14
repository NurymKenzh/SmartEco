using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredParameter_20190427_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MPC",
                table: "MeasuredParameter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MPC",
                table: "MeasuredParameter");
        }
    }
}
