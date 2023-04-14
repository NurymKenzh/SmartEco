using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class SpeciallyProtectedNaturalTerritory_20201028_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpeciallyProtectedNaturalTerritory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true),
                    NameEN = table.Column<string>(nullable: true),
                    AuthorizedAuthorityId = table.Column<int>(nullable: false),
                    Areahectares = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciallyProtectedNaturalTerritory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeciallyProtectedNaturalTerritory_AuthorizedAuthority_Auth~",
                        column: x => x.AuthorizedAuthorityId,
                        principalTable: "AuthorizedAuthority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeciallyProtectedNaturalTerritory_AuthorizedAuthorityId",
                table: "SpeciallyProtectedNaturalTerritory",
                column: "AuthorizedAuthorityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeciallyProtectedNaturalTerritory");
        }
    }
}
