using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class PollutantAir_20231010_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PollutantAir",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Code = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Formula = table.Column<string>(nullable: true),
                    MpcMaxSingle = table.Column<decimal>(nullable: true),
                    MpcAvgDaily = table.Column<decimal>(nullable: true),
                    Asel = table.Column<decimal>(nullable: true),
                    HazardLevelId = table.Column<int>(nullable: false),
                    MeasuredUnit = table.Column<string>(nullable: true),
                    Cas = table.Column<string>(nullable: true),
                    MpcMaxSingle2 = table.Column<decimal>(nullable: true),
                    SummationGroup = table.Column<string>(nullable: true),
                    AggregationState = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollutantAir", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollutantAir_HazardLevel_HazardLevelId",
                        column: x => x.HazardLevelId,
                        principalSchema: "asm",
                        principalTable: "HazardLevel",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollutantAir_HazardLevelId",
                schema: "asm",
                table: "PollutantAir",
                column: "HazardLevelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollutantAir",
                schema: "asm");
        }
    }
}
