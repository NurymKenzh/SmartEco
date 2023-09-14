using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutionSourceRelations_20230908_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirPollutionSourceArea",
                schema: "asm",
                columns: table => new
                {
                    AirPollutionSourceId = table.Column<int>(nullable: false),
                    AreaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirPollutionSourceArea", x => new { x.AirPollutionSourceId, x.AreaId });
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceArea_AirPollutionSource_AirPollutionSourc~",
                        column: x => x.AirPollutionSourceId,
                        principalSchema: "asm",
                        principalTable: "AirPollutionSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceArea_Area_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "asm",
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirPollutionSourceIndSite",
                schema: "asm",
                columns: table => new
                {
                    AirPollutionSourceId = table.Column<int>(nullable: false),
                    IndSiteEnterpriseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirPollutionSourceIndSite", x => new { x.AirPollutionSourceId, x.IndSiteEnterpriseId });
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceIndSite_AirPollutionSource_AirPollutionSo~",
                        column: x => x.AirPollutionSourceId,
                        principalSchema: "asm",
                        principalTable: "AirPollutionSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceIndSite_IndSiteEnterprise_IndSiteEnterpri~",
                        column: x => x.IndSiteEnterpriseId,
                        principalSchema: "asm",
                        principalTable: "IndSiteEnterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AirPollutionSourceWorkshop",
                schema: "asm",
                columns: table => new
                {
                    AirPollutionSourceId = table.Column<int>(nullable: false),
                    WorkshopId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirPollutionSourceWorkshop", x => new { x.AirPollutionSourceId, x.WorkshopId });
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceWorkshop_AirPollutionSource_AirPollutionS~",
                        column: x => x.AirPollutionSourceId,
                        principalSchema: "asm",
                        principalTable: "AirPollutionSource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AirPollutionSourceWorkshop_Workshop_WorkshopId",
                        column: x => x.WorkshopId,
                        principalSchema: "asm",
                        principalTable: "Workshop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceArea_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceArea",
                column: "AirPollutionSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceArea_AreaId",
                schema: "asm",
                table: "AirPollutionSourceArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceIndSite_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceIndSite",
                column: "AirPollutionSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceIndSite_IndSiteEnterpriseId",
                schema: "asm",
                table: "AirPollutionSourceIndSite",
                column: "IndSiteEnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceWorkshop_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceWorkshop",
                column: "AirPollutionSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceWorkshop_WorkshopId",
                schema: "asm",
                table: "AirPollutionSourceWorkshop",
                column: "WorkshopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirPollutionSourceArea",
                schema: "asm");

            migrationBuilder.DropTable(
                name: "AirPollutionSourceIndSite",
                schema: "asm");

            migrationBuilder.DropTable(
                name: "AirPollutionSourceWorkshop",
                schema: "asm");
        }
    }
}
