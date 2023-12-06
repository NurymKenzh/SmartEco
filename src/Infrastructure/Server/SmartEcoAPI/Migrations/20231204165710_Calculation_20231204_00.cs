using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class Calculation_20231204_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calculation",
                schema: "asmUprza",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    TypeId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    KatoCode = table.Column<string>(nullable: true),
                    KatoName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calculation_CalculationStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "asmUprza",
                        principalTable: "CalculationStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Calculation_CalculationType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "asmUprza",
                        principalTable: "CalculationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calculation_StatusId",
                schema: "asmUprza",
                table: "Calculation",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Calculation_TypeId",
                schema: "asmUprza",
                table: "Calculation",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculation",
                schema: "asmUprza");
        }
    }
}
