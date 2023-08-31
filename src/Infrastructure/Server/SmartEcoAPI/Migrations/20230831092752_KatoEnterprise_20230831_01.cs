using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class KatoEnterprise_20230831_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KatoEnterprise",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Code = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    EnterpriseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KatoEnterprise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KatoEnterprise_Enterprise_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalSchema: "asm",
                        principalTable: "Enterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KatoEnterprise_EnterpriseId",
                schema: "asm",
                table: "KatoEnterprise",
                column: "EnterpriseId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KatoEnterprise",
                schema: "asm");
        }
    }
}
