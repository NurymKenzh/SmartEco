using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class TargetTerritory_20200214_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TargetTerritory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TerritoryTypeId = table.Column<int>(nullable: false),
                    KATOId = table.Column<int>(nullable: true),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true),
                    GISConnectionCode = table.Column<string>(nullable: true),
                    AdditionalInformationKK = table.Column<string>(nullable: true),
                    AdditionalInformationRU = table.Column<string>(nullable: true),
                    KazHydrometAirPostId = table.Column<int>(nullable: true),
                    MonitoringPostId = table.Column<int>(nullable: true),
                    KazHydrometSoilPostId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetTerritory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetTerritory_KATO_KATOId",
                        column: x => x.KATOId,
                        principalTable: "KATO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetTerritory_KazHydrometAirPost_KazHydrometAirPostId",
                        column: x => x.KazHydrometAirPostId,
                        principalTable: "KazHydrometAirPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetTerritory_KazHydrometSoilPost_KazHydrometSoilPostId",
                        column: x => x.KazHydrometSoilPostId,
                        principalTable: "KazHydrometSoilPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetTerritory_MonitoringPost_MonitoringPostId",
                        column: x => x.MonitoringPostId,
                        principalTable: "MonitoringPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TargetTerritory_TerritoryType_TerritoryTypeId",
                        column: x => x.TerritoryTypeId,
                        principalTable: "TerritoryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_KATOId",
                table: "TargetTerritory",
                column: "KATOId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_KazHydrometAirPostId",
                table: "TargetTerritory",
                column: "KazHydrometAirPostId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_KazHydrometSoilPostId",
                table: "TargetTerritory",
                column: "KazHydrometSoilPostId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_MonitoringPostId",
                table: "TargetTerritory",
                column: "MonitoringPostId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_TerritoryTypeId",
                table: "TargetTerritory",
                column: "TerritoryTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TargetTerritory");
        }
    }
}
