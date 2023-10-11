using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutant_20231011_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirPollutant_HazardLevel_HazardLevelId",
                schema: "asm",
                table: "AirPollutant");

            migrationBuilder.AlterColumn<int>(
                name: "HazardLevelId",
                schema: "asm",
                table: "AirPollutant",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AirPollutant_HazardLevel_HazardLevelId",
                schema: "asm",
                table: "AirPollutant",
                column: "HazardLevelId",
                principalSchema: "asm",
                principalTable: "HazardLevel",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirPollutant_HazardLevel_HazardLevelId",
                schema: "asm",
                table: "AirPollutant");

            migrationBuilder.AlterColumn<int>(
                name: "HazardLevelId",
                schema: "asm",
                table: "AirPollutant",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AirPollutant_HazardLevel_HazardLevelId",
                schema: "asm",
                table: "AirPollutant",
                column: "HazardLevelId",
                principalSchema: "asm",
                principalTable: "HazardLevel",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
