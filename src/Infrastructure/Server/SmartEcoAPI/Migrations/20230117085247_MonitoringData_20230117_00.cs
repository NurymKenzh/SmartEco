using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class MonitoringData_20230117_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitoringData",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    MonitoringParameterId = table.Column<int>(nullable: false),
                    SourceEmissionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringData_MonitoringParameter_MonitoringParameterId",
                        column: x => x.MonitoringParameterId,
                        principalSchema: "asm",
                        principalTable: "MonitoringParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitoringData_SourceEmission_SourceEmissionId",
                        column: x => x.SourceEmissionId,
                        principalSchema: "asm",
                        principalTable: "SourceEmission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringData_MonitoringParameterId",
                schema: "asm",
                table: "MonitoringData",
                column: "MonitoringParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringData_SourceEmissionId",
                schema: "asm",
                table: "MonitoringData",
                column: "SourceEmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoringData",
                schema: "asm");
        }
    }
}
