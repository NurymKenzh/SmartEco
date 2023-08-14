using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class IndSiteEnterpriseBorder_20230808_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndSiteEnterpriseBorder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    IndSiteEnterpriseId = table.Column<int>(nullable: false),
                    Coordinates = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndSiteEnterpriseBorder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndSiteEnterpriseBorder_IndSiteEnterprise_IndSiteEnterprise~",
                        column: x => x.IndSiteEnterpriseId,
                        principalSchema: "asm",
                        principalTable: "IndSiteEnterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndSiteEnterpriseBorder_IndSiteEnterpriseId",
                table: "IndSiteEnterpriseBorder",
                column: "IndSiteEnterpriseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndSiteEnterpriseBorder");
        }
    }
}
