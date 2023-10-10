using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class GasAirMixture_20231010_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GasAirMixture",
                schema: "asm",
                columns: table => new
                {
                    OperationModeId = table.Column<int>(nullable: false),
                    Temperature = table.Column<double>(nullable: false),
                    Pressure = table.Column<double>(nullable: false),
                    Speed = table.Column<double>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    Humidity = table.Column<double>(nullable: false),
                    Density = table.Column<double>(nullable: false),
                    ThermalPower = table.Column<double>(nullable: false),
                    PartRadiation = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GasAirMixture", x => x.OperationModeId);
                    table.ForeignKey(
                        name: "FK_GasAirMixture_OperationMode_OperationModeId",
                        column: x => x.OperationModeId,
                        principalSchema: "asm",
                        principalTable: "OperationMode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GasAirMixture",
                schema: "asm");
        }
    }
}
