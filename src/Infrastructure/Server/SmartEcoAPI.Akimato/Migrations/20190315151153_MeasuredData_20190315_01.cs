using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredData_20190315_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_EcomonMonitoringPoint_EcomonMonitoringPointId",
                table: "MeasuredData");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_KazHydrometAirPost_KazHydrometAirPostId",
                table: "MeasuredData");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_EcomonMonitoringPointId",
                table: "MeasuredData");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_KazHydrometAirPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "EcomonMonitoringPointId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "KazHydrometAirPostId",
                table: "MeasuredData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EcomonMonitoringPointId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KazHydrometAirPostId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_EcomonMonitoringPointId",
                table: "MeasuredData",
                column: "EcomonMonitoringPointId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_KazHydrometAirPostId",
                table: "MeasuredData",
                column: "KazHydrometAirPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredData_EcomonMonitoringPoint_EcomonMonitoringPointId",
                table: "MeasuredData",
                column: "EcomonMonitoringPointId",
                principalTable: "EcomonMonitoringPoint",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredData_KazHydrometAirPost_KazHydrometAirPostId",
                table: "MeasuredData",
                column: "KazHydrometAirPostId",
                principalTable: "KazHydrometAirPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
