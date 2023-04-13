using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class LEDSreen_20200615_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "LEDScreen",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "LEDScreen");
        }
    }
}
