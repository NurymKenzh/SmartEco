using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class Borders_20230831_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SanZoneEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "SanZoneEnterpriseBorder");

            migrationBuilder.DropIndex(
                name: "IX_IndSiteEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "IndSiteEnterpriseBorder");

            migrationBuilder.CreateIndex(
                name: "IX_SanZoneEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "SanZoneEnterpriseBorder",
                column: "IndSiteEnterpriseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndSiteEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "IndSiteEnterpriseBorder",
                column: "IndSiteEnterpriseId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SanZoneEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "SanZoneEnterpriseBorder");

            migrationBuilder.DropIndex(
                name: "IX_IndSiteEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "IndSiteEnterpriseBorder");

            migrationBuilder.CreateIndex(
                name: "IX_SanZoneEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "SanZoneEnterpriseBorder",
                column: "IndSiteEnterpriseId");

            migrationBuilder.CreateIndex(
                name: "IX_IndSiteEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "IndSiteEnterpriseBorder",
                column: "IndSiteEnterpriseId");
        }
    }
}
