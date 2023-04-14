using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class AActivity_20200220_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AActivity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TargetId = table.Column<int>(nullable: false),
                    TargetTerritoryId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    ActivityType = table.Column<bool>(nullable: false),
                    ImplementationPercentage = table.Column<decimal>(nullable: false),
                    AdditionalInformationKK = table.Column<string>(nullable: true),
                    AdditionalInformationRU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AActivity_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AActivity_Target_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Target",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AActivity_TargetTerritory_TargetTerritoryId",
                        column: x => x.TargetTerritoryId,
                        principalTable: "TargetTerritory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AActivity_EventId",
                table: "AActivity",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AActivity_TargetId",
                table: "AActivity",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_AActivity_TargetTerritoryId",
                table: "AActivity",
                column: "TargetTerritoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AActivity");
        }
    }
}
