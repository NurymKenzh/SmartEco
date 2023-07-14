using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class Enterprise_20230713_00 : Migration
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
                    Bin = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    KatoId = table.Column<int>(nullable: false),
                    EnterpriseTypeId = table.Column<int>(nullable: true),
                    Contacts = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enterprise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enterprise_EnterpriseType_EnterpriseTypeId",
                        column: x => x.EnterpriseTypeId,
                        principalSchema: "asm",
                        principalTable: "EnterpriseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enterprise_EnterpriseTypeId",
                schema: "asm",
                table: "Enterprise",
                column: "EnterpriseTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enterprise",
                schema: "asm");
        }
    }
}
