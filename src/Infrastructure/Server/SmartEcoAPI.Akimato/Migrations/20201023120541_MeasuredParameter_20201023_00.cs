using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredParameter_20201023_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PollutionEnvironmentId",
                table: "MeasuredParameter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredParameter_PollutionEnvironmentId",
                table: "MeasuredParameter",
                column: "PollutionEnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredParameter_PollutionEnvironment_PollutionEnvironment~",
                table: "MeasuredParameter",
                column: "PollutionEnvironmentId",
                principalTable: "PollutionEnvironment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredParameter_PollutionEnvironment_PollutionEnvironment~",
                table: "MeasuredParameter");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredParameter_PollutionEnvironmentId",
                table: "MeasuredParameter");

            migrationBuilder.DropColumn(
                name: "PollutionEnvironmentId",
                table: "MeasuredParameter");
        }
    }
}
