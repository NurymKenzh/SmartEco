using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class MonitoringPostMeasuredParameters_20190917_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MonitoringPostMeasuredParameters_MeasuredParameterId",
                table: "MonitoringPostMeasuredParameters",
                column: "MeasuredParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringPostMeasuredParameters_MonitoringPostId",
                table: "MonitoringPostMeasuredParameters",
                column: "MonitoringPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringPostMeasuredParameters_MeasuredParameter_Measured~",
                table: "MonitoringPostMeasuredParameters",
                column: "MeasuredParameterId",
                principalTable: "MeasuredParameter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoringPostMeasuredParameters_MonitoringPost_MonitoringP~",
                table: "MonitoringPostMeasuredParameters",
                column: "MonitoringPostId",
                principalTable: "MonitoringPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringPostMeasuredParameters_MeasuredParameter_Measured~",
                table: "MonitoringPostMeasuredParameters");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoringPostMeasuredParameters_MonitoringPost_MonitoringP~",
                table: "MonitoringPostMeasuredParameters");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringPostMeasuredParameters_MeasuredParameterId",
                table: "MonitoringPostMeasuredParameters");

            migrationBuilder.DropIndex(
                name: "IX_MonitoringPostMeasuredParameters_MonitoringPostId",
                table: "MonitoringPostMeasuredParameters");
        }
    }
}
