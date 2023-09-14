using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutionSourceInfo_20230911_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirPollutionSourceInfo",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Coordinate = table.Column<string>(nullable: true),
                    TerrainCoefficient = table.Column<int>(nullable: false),
                    IsCalculateByGas = table.Column<bool>(nullable: false),
                    IsVerticalDeviation = table.Column<bool>(nullable: false),
                    AngleDeflection = table.Column<decimal>(nullable: false),
                    AngleRotation = table.Column<decimal>(nullable: false),
                    IsCovered = table.Column<bool>(nullable: false),
                    IsSignFlare = table.Column<bool>(nullable: false),
                    Hight = table.Column<decimal>(nullable: false),
                    Diameter = table.Column<decimal>(nullable: false),
                    RelationBackground = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirPollutionSourceInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceInfo_AirPollutionSource_SourceId",
                        column: x => x.SourceId,
                        principalSchema: "asm",
                        principalTable: "AirPollutionSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceInfo_SourceId",
                schema: "asm",
                table: "AirPollutionSourceInfo",
                column: "SourceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirPollutionSourceInfo",
                schema: "asm");
        }
    }
}
