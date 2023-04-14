using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredData_20190315_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonitoringPostId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_MonitoringPostId",
                table: "MeasuredData",
                column: "MonitoringPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredData_MonitoringPost_MonitoringPostId",
                table: "MeasuredData",
                column: "MonitoringPostId",
                principalTable: "MonitoringPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_MonitoringPost_MonitoringPostId",
                table: "MeasuredData");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_MonitoringPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "MonitoringPostId",
                table: "MeasuredData");
        }
    }
}
