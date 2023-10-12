using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AirEmission_20231011_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationModeId",
                schema: "asm",
                table: "AirEmission",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AirEmission_OperationModeId",
                schema: "asm",
                table: "AirEmission",
                column: "OperationModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AirEmission_OperationMode_OperationModeId",
                schema: "asm",
                table: "AirEmission",
                column: "OperationModeId",
                principalSchema: "asm",
                principalTable: "OperationMode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirEmission_OperationMode_OperationModeId",
                schema: "asm",
                table: "AirEmission");

            migrationBuilder.DropIndex(
                name: "IX_AirEmission_OperationModeId",
                schema: "asm",
                table: "AirEmission");

            migrationBuilder.DropColumn(
                name: "OperationModeId",
                schema: "asm",
                table: "AirEmission");
        }
    }
}
