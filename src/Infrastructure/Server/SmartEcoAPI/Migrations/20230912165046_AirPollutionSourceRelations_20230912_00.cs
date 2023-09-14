using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AirPollutionSourceRelations_20230912_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceWorkshop",
                schema: "asm",
                table: "AirPollutionSourceWorkshop");

            migrationBuilder.DropIndex(
                name: "IX_AirPollutionSourceWorkshop_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceWorkshop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceIndSite",
                schema: "asm",
                table: "AirPollutionSourceIndSite");

            migrationBuilder.DropIndex(
                name: "IX_AirPollutionSourceIndSite_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceIndSite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceArea",
                schema: "asm",
                table: "AirPollutionSourceArea");

            migrationBuilder.DropIndex(
                name: "IX_AirPollutionSourceArea_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceArea");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceWorkshop",
                schema: "asm",
                table: "AirPollutionSourceWorkshop",
                column: "AirPollutionSourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceIndSite",
                schema: "asm",
                table: "AirPollutionSourceIndSite",
                column: "AirPollutionSourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceArea",
                schema: "asm",
                table: "AirPollutionSourceArea",
                column: "AirPollutionSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceWorkshop",
                schema: "asm",
                table: "AirPollutionSourceWorkshop");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceIndSite",
                schema: "asm",
                table: "AirPollutionSourceIndSite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AirPollutionSourceArea",
                schema: "asm",
                table: "AirPollutionSourceArea");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceWorkshop",
                schema: "asm",
                table: "AirPollutionSourceWorkshop",
                columns: new[] { "AirPollutionSourceId", "WorkshopId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceIndSite",
                schema: "asm",
                table: "AirPollutionSourceIndSite",
                columns: new[] { "AirPollutionSourceId", "IndSiteEnterpriseId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AirPollutionSourceArea",
                schema: "asm",
                table: "AirPollutionSourceArea",
                columns: new[] { "AirPollutionSourceId", "AreaId" });

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceWorkshop_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceWorkshop",
                column: "AirPollutionSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceIndSite_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceIndSite",
                column: "AirPollutionSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirPollutionSourceArea_AirPollutionSourceId",
                schema: "asm",
                table: "AirPollutionSourceArea",
                column: "AirPollutionSourceId",
                unique: true);
        }
    }
}
