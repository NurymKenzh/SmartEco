using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class IndSiteEnterprise_20230714_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndSiteEnterprise",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    EnterpriseId = table.Column<int>(nullable: false),
                    MinSizeSanitaryZone = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndSiteEnterprise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndSiteEnterprise_Enterprise_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalSchema: "asm",
                        principalTable: "Enterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndSiteEnterprise_EnterpriseId",
                schema: "asm",
                table: "IndSiteEnterprise",
                column: "EnterpriseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndSiteEnterprise",
                schema: "asm");
        }
    }
}
