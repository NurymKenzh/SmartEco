using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class SpeciesDiversity_20201029_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpeciesDiversity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    KATOId = table.Column<int>(nullable: false),
                    PlantationsTypeId = table.Column<int>(nullable: false),
                    TreesNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesDiversity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeciesDiversity_KATO_KATOId",
                        column: x => x.KATOId,
                        principalTable: "KATO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeciesDiversity_PlantationsType_PlantationsTypeId",
                        column: x => x.PlantationsTypeId,
                        principalTable: "PlantationsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesDiversity_KATOId",
                table: "SpeciesDiversity",
                column: "KATOId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesDiversity_PlantationsTypeId",
                table: "SpeciesDiversity",
                column: "PlantationsTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeciesDiversity");
        }
    }
}
