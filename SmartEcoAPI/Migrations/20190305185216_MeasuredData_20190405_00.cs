using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class MeasuredData_20190405_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KazHydrometAirPostId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_KazHydrometAirPostId",
                table: "MeasuredData",
                column: "KazHydrometAirPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredData_KazHydrometAirPost_KazHydrometAirPostId",
                table: "MeasuredData",
                column: "KazHydrometAirPostId",
                principalTable: "KazHydrometAirPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_KazHydrometAirPost_KazHydrometAirPostId",
                table: "MeasuredData");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_KazHydrometAirPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "KazHydrometAirPostId",
                table: "MeasuredData");
        }
    }
}
