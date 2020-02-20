using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class TargetTerritory_20200214_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TargetTerritory_KazHydrometAirPost_KazHydrometAirPostId",
                table: "TargetTerritory");

            migrationBuilder.DropIndex(
                name: "IX_TargetTerritory_KazHydrometAirPostId",
                table: "TargetTerritory");

            migrationBuilder.DropColumn(
                name: "KazHydrometAirPostId",
                table: "TargetTerritory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KazHydrometAirPostId",
                table: "TargetTerritory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_KazHydrometAirPostId",
                table: "TargetTerritory",
                column: "KazHydrometAirPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTerritory_KazHydrometAirPost_KazHydrometAirPostId",
                table: "TargetTerritory",
                column: "KazHydrometAirPostId",
                principalTable: "KazHydrometAirPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
