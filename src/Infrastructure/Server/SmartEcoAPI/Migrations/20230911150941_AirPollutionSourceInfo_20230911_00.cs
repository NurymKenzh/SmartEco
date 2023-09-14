using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutionSourceInfo_20230911_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceInfo",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.DropIndex(
                name: "IX_AirPollutionSourceInfo_SourceId",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceInfo",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceInfo",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceInfo",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceInfo_SourceId",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                column: "SourceId",
                unique: true);
        }
    }
}
