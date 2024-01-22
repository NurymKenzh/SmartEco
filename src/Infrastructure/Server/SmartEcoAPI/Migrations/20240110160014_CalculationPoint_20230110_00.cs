using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class CalculationPoint_20230110_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationPoint",
                schema: "asmUprza",
                columns: table => new
                {
                    CalculationId = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    AbscissaX = table.Column<double>(nullable: false),
                    OrdinateY = table.Column<double>(nullable: false),
                    ApplicateZ = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationPoint", x => new { x.CalculationId, x.Number });
                    table.ForeignKey(
                        name: "FK_CalculationPoint_Calculation_CalculationId",
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
                name: "CalculationPoint",
                schema: "asmUprza");
        }
    }
}
