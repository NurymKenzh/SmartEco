using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class Enterprise_20230112_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Enterprise",
                schema: "asm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    NorthLatitude = table.Column<decimal>(nullable: false),
                    EastLongitude = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enterprise_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "asm",
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enterprise_CompanyId",
                schema: "asm",
                table: "Enterprise",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enterprise",
                schema: "asm");
        }
    }
}
