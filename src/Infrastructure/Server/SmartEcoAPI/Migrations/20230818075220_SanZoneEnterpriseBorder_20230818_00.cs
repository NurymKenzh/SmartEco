using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class SanZoneEnterpriseBorder_20230818_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SanZoneEnterpriseBorder",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    PermissibleConcentration = table.Column<decimal>(nullable: false),
                    IndSiteEnterpriseId = table.Column<int>(nullable: false),
                    Coordinates = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanZoneEnterpriseBorder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SanZoneEnterpriseBorder_IndSiteEnterprise_IndSiteEnterprise~",
                        column: x => x.IndSiteEnterpriseId,
                        principalSchema: "asm",
                        principalTable: "IndSiteEnterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SanZoneEnterpriseBorder_IndSiteEnterpriseId",
                schema: "asm",
                table: "SanZoneEnterpriseBorder",
                column: "IndSiteEnterpriseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SanZoneEnterpriseBorder",
                schema: "asm");
        }
    }
}
