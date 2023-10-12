using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class AirEmission_20231011_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirEmission",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PollutantId = table.Column<int>(nullable: false),
                    MaxGramSec = table.Column<decimal>(nullable: false),
                    MaxMilligramMeter = table.Column<decimal>(nullable: true),
                    GrossTonYear = table.Column<decimal>(nullable: false),
                    SettlingCoef = table.Column<int>(nullable: false),
                    EnteredDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirEmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirEmission_AirPollutant_PollutantId",
                        column: x => x.PollutantId,
                        principalSchema: "asm",
                        principalTable: "AirPollutant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirEmission_PollutantId",
                schema: "asm",
                table: "AirEmission",
                column: "PollutantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirEmission",
                schema: "asm");
        }
    }
}
