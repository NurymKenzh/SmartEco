using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class StateCalculation_20240122_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StateCalculation",
                schema: "asmUprza",
                columns: table => new
                {
                    CalculationId = table.Column<int>(nullable: false),
                    JobId = table.Column<int>(nullable: false),
                    InitializedOn = table.Column<DateTime>(nullable: false),
                    Description = table.Column<List<string>>(nullable: true),
                    DiagnosticInfo_Progress = table.Column<int>(nullable: false),
                    DiagnosticInfo_CalculationTime = table.Column<double>(nullable: false),
                    DiagnosticInfo_AverageTime = table.Column<double>(nullable: false),
                    DiagnosticInfo_NumberOfPoints = table.Column<int>(nullable: false),
                    DiagnosticInfo_NumberOfThreads = table.Column<int>(nullable: false),
                    DiagnosticInfo_NumberOfIterations = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateCalculation", x => x.CalculationId);
                    table.ForeignKey(
                        name: "FK_StateCalculation_Calculation_CalculationId",
                        column: x => x.CalculationId,
                        principalSchema: "asmUprza",
                        principalTable: "Calculation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StateCalculation",
                schema: "asmUprza");
        }
    }
}
