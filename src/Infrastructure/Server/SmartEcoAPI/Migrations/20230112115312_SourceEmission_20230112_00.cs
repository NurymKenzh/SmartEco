using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class SourceEmission_20230112_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SourceEmission",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    NorthLatitude = table.Column<decimal>(nullable: false),
                    EastLongitude = table.Column<decimal>(nullable: false),
                    SourceAirPollutionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceEmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SourceEmission_SourceAirPollution_SourceAirPollutionId",
                        column: x => x.SourceAirPollutionId,
                        principalSchema: "asm",
                        principalTable: "SourceAirPollution",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SourceEmission_SourceAirPollutionId",
                schema: "asm",
                table: "SourceEmission",
                column: "SourceAirPollutionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceEmission",
                schema: "asm");
        }
    }
}
