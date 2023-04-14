using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MeasuredData_20190105_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasuredData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    MeasuredParameterId = table.Column<int>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    EcomonMonitoringPointId = table.Column<int>(nullable: true),
                    Ecomontimestamp_ms = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasuredData_EcomonMonitoringPoint_EcomonMonitoringPointId",
                        column: x => x.EcomonMonitoringPointId,
                        principalTable: "EcomonMonitoringPoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasuredData_MeasuredParameter_MeasuredParameterId",
                        column: x => x.MeasuredParameterId,
                        principalTable: "MeasuredParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_EcomonMonitoringPointId",
                table: "MeasuredData",
                column: "EcomonMonitoringPointId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_MeasuredParameterId",
                table: "MeasuredData",
                column: "MeasuredParameterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasuredData");
        }
    }
}
