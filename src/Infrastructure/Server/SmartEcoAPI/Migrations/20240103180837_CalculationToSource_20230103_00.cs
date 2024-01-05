using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class CalculationToSource_20230103_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationToSource",
                schema: "asmUprza",
                columns: table => new
                {
                    CalculationId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationToSource", x => new { x.CalculationId, x.SourceId });
                    table.ForeignKey(
                        name: "FK_CalculationToSource_Calculation_CalculationId",
                        column: x => x.CalculationId,
                        principalSchema: "asmUprza",
                        principalTable: "Calculation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalculationToSource_AirPollutionSource_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "asm",
                        principalTable: "AirPollutionSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationToSource_SourceId",
                schema: "asmUprza",
                table: "CalculationToSource",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationToSource",
                schema: "asmUprza");
        }
    }
}
