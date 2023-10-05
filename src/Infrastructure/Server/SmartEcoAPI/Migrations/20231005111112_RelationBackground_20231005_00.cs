using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class RelationBackground_20231005_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelationBackground",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.AddColumn<int>(
                name: "RelationBackgroundId",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RelationBackground",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationBackground", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceInfo_RelationBackgroundId",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                column: "RelationBackgroundId");

            migrationBuilder.AddForeignKey(
                name: "FK_AirPollutionSourceInfo_RelationBackground_RelationBackgroun~",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                column: "RelationBackgroundId",
                principalSchema: "asm",
                principalTable: "RelationBackground",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirPollutionSourceInfo_RelationBackground_RelationBackgroun~",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.DropTable(
                name: "RelationBackground",
                schema: "asm");

            migrationBuilder.DropIndex(
                name: "IX_AirPollutionSourceInfo_RelationBackgroundId",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.DropColumn(
                name: "RelationBackgroundId",
                schema: "asm",
                table: "AirPollutionSourceInfo");

            migrationBuilder.AddColumn<int>(
                name: "RelationBackground",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                nullable: false,
                defaultValue: 0);
        }
    }
}
