using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredData_20190401_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EastLongitude",
                table: "PollutionSource",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NorthLatitude",
                table: "PollutionSource",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PollutionSourceId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_PollutionSourceId",
                table: "MeasuredData",
                column: "PollutionSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredData_PollutionSource_PollutionSourceId",
                table: "MeasuredData",
                column: "PollutionSourceId",
                principalTable: "PollutionSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_PollutionSource_PollutionSourceId",
                table: "MeasuredData");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_PollutionSourceId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "EastLongitude",
                table: "PollutionSource");

            migrationBuilder.DropColumn(
                name: "NorthLatitude",
                table: "PollutionSource");

            migrationBuilder.DropColumn(
                name: "PollutionSourceId",
                table: "MeasuredData");
        }
    }
}
