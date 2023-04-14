using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class TargetValue_20200219_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TargetValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TargetId = table.Column<int>(nullable: false),
                    TargetTerritoryId = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    TargetValueType = table.Column<bool>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    AdditionalInformationKK = table.Column<string>(nullable: true),
                    AdditionalInformationRU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetValue_Target_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Target",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetValue_TargetTerritory_TargetTerritoryId",
                        column: x => x.TargetTerritoryId,
                        principalTable: "TargetTerritory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TargetValue_TargetId",
                table: "TargetValue",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetValue_TargetTerritoryId",
                table: "TargetValue",
                column: "TargetTerritoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TargetValue");
        }
    }
}
