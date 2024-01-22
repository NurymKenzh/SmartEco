using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class CalculationSetting_20240112_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationSetting",
                schema: "asmUprza",
                columns: table => new
                {
                    CalculationId = table.Column<int>(nullable: false),
                    SettingsJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationSetting", x => x.CalculationId);
                    table.ForeignKey(
                        name: "FK_CalculationSetting_Calculation_CalculationId",
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
                name: "CalculationSetting",
                schema: "asmUprza");
        }
    }
}
