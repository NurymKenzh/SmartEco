using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class PollutionSourceData_20190130_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollutionSourceData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PollutantId = table.Column<int>(nullable: false),
                    PollutionSourcetId = table.Column<int>(nullable: false),
                    PollutionSourceId = table.Column<int>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollutionSourceData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollutionSourceData_Pollutant_PollutantId",
                        column: x => x.PollutantId,
                        principalTable: "Pollutant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollutionSourceData_PollutionSource_PollutionSourceId",
                        column: x => x.PollutionSourceId,
                        principalTable: "PollutionSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollutionSourceData_PollutantId",
                table: "PollutionSourceData",
                column: "PollutantId");

            migrationBuilder.CreateIndex(
                name: "IX_PollutionSourceData_PollutionSourceId",
                table: "PollutionSourceData",
                column: "PollutionSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollutionSourceData");
        }
    }
}
