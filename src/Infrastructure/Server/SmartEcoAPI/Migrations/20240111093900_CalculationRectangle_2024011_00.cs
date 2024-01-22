using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class CalculationRectangle_2024011_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationRectangle",
                schema: "asmUprza",
                columns: table => new
                {
                    CalculationId = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    AbscissaX = table.Column<double>(nullable: false),
                    OrdinateY = table.Column<double>(nullable: false),
                    ApplicateZ = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    StepByWidth = table.Column<int>(nullable: false),
                    StepByLength = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationRectangle", x => new { x.CalculationId, x.Number });
                    table.ForeignKey(
                        name: "FK_CalculationRectangle_Calculation_CalculationId",
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
                name: "CalculationRectangle",
                schema: "asmUprza");
        }
    }
}
