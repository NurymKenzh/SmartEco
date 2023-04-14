using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredData_20190306_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxValuePerMonth",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValuePerYear",
                table: "MeasuredData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxValuePerMonth",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "MaxValuePerYear",
                table: "MeasuredData");
        }
    }
}
