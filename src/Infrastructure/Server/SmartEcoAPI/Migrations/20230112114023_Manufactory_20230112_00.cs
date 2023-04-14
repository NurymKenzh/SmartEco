using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class Manufactory_20230112_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufactory",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    NorthLatitude = table.Column<decimal>(nullable: false),
                    EastLongitude = table.Column<decimal>(nullable: false),
                    EnterpriseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufactory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manufactory_Enterprise_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalSchema: "asm",
                        principalTable: "Enterprise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Manufactory_EnterpriseId",
                schema: "asm",
                table: "Manufactory",
                column: "EnterpriseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Manufactory",
                schema: "asm");
        }
    }
}
