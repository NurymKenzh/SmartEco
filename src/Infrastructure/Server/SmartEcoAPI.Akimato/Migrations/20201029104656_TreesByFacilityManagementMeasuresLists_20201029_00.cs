using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class TreesByFacilityManagementMeasuresLists_20201029_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreesByFacilityManagementMeasuresList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GreemPlantsPassportId = table.Column<int>(nullable: false),
                    PlantationsTypeId = table.Column<int>(nullable: false),
                    BusinessEventsPlantationsTypeId = table.Column<int>(nullable: true),
                    SanitaryPruning = table.Column<int>(nullable: true),
                    CrownFormation = table.Column<int>(nullable: true),
                    SanitaryFelling = table.Column<int>(nullable: true),
                    MaintenanceWork = table.Column<string>(nullable: true),
                    Quantity = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreesByFacilityManagementMeasuresList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreesByFacilityManagementMeasuresList_PlantationsType_Busin~",
                        column: x => x.BusinessEventsPlantationsTypeId,
                        principalTable: "PlantationsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreesByFacilityManagementMeasuresList_GreemPlantsPassport_G~",
                        column: x => x.GreemPlantsPassportId,
                        principalTable: "GreemPlantsPassport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreesByFacilityManagementMeasuresList_PlantationsType_Plant~",
                        column: x => x.PlantationsTypeId,
                        principalTable: "PlantationsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreesByFacilityManagementMeasuresList_BusinessEventsPlantat~",
                table: "TreesByFacilityManagementMeasuresList",
                column: "BusinessEventsPlantationsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TreesByFacilityManagementMeasuresList_GreemPlantsPassportId",
                table: "TreesByFacilityManagementMeasuresList",
                column: "GreemPlantsPassportId");

            migrationBuilder.CreateIndex(
                name: "IX_TreesByFacilityManagementMeasuresList_PlantationsTypeId",
                table: "TreesByFacilityManagementMeasuresList",
                column: "PlantationsTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreesByFacilityManagementMeasuresList");
        }
    }
}
