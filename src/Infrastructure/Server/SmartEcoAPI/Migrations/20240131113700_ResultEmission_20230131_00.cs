using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class ResultEmission_20230131_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultEmission",
                schema: "asmUprza",
                columns: table => new
                {
                    CalculationId = table.Column<int>(nullable: false),
                    AirPollutantId = table.Column<int>(nullable: false),
                    FeatureCollection = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultEmission", x => new { x.CalculationId, x.AirPollutantId });
                    table.ForeignKey(
                        name: "FK_ResultEmission_AirPollutant_AirPollutantId",
                        column: x => x.AirPollutantId,
                        principalSchema: "asm",
                        principalTable: "AirPollutant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultEmission_Calculation_CalculationId",
                        column: x => x.CalculationId,
                        principalSchema: "asmUprza",
                        principalTable: "Calculation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResultEmission_AirPollutantId",
                schema: "asmUprza",
                table: "ResultEmission",
                column: "AirPollutantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultEmission",
                schema: "asmUprza");
        }
    }
}
