using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredParameter_20200616_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameShortEN",
                table: "MeasuredParameter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameShortKK",
                table: "MeasuredParameter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameShortRU",
                table: "MeasuredParameter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameShortEN",
                table: "MeasuredParameter");

            migrationBuilder.DropColumn(
                name: "NameShortKK",
                table: "MeasuredParameter");

            migrationBuilder.DropColumn(
                name: "NameShortRU",
                table: "MeasuredParameter");
        }
    }
}
