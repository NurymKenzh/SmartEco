using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class MonitoringPost_20191223_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "MonitoringPost",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringPost_ProjectId",
                table: "MonitoringPost",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringPost_Project_ProjectId",
                table: "MonitoringPost",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringPost_Project_ProjectId",
                table: "MonitoringPost");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringPost_ProjectId",
                table: "MonitoringPost");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "MonitoringPost");
        }
    }
}
