using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class PlantationsState_20201029_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantationsState",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    KATOId = table.Column<int>(nullable: false),
                    PlantationsStateTypeId = table.Column<int>(nullable: false),
                    TreesNumber = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantationsState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantationsState_KATO_KATOId",
                        column: x => x.KATOId,
                        principalTable: "KATO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantationsState_PlantationsStateType_PlantationsStateTypeId",
                        column: x => x.PlantationsStateTypeId,
                        principalTable: "PlantationsStateType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantationsState_KATOId",
                table: "PlantationsState",
                column: "KATOId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantationsState_PlantationsStateTypeId",
                table: "PlantationsState",
                column: "PlantationsStateTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlantationsState");
        }
    }
}
