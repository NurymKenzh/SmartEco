using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class PollutionSourceData_20190130_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollutionSourceData_PollutionSource_PollutionSourceId",
                table: "PollutionSourceData");

            migrationBuilder.DropColumn(
                name: "PollutionSourcetId",
                table: "PollutionSourceData");

            migrationBuilder.AlterColumn<int>(
                name: "PollutionSourceId",
                table: "PollutionSourceData",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PollutionSourceData_PollutionSource_PollutionSourceId",
                table: "PollutionSourceData",
                column: "PollutionSourceId",
                principalTable: "PollutionSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PollutionSourceData_PollutionSource_PollutionSourceId",
                table: "PollutionSourceData");

            migrationBuilder.AlterColumn<int>(
                name: "PollutionSourceId",
                table: "PollutionSourceData",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PollutionSourcetId",
                table: "PollutionSourceData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PollutionSourceData_PollutionSource_PollutionSourceId",
                table: "PollutionSourceData",
                column: "PollutionSourceId",
                principalTable: "PollutionSource",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
