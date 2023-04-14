using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class TreesByObjectTableOfTheBreedStateList_20201028_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TreesByObjectTableOfTheBreedStateList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GreemPlantsPassportId = table.Column<int>(nullable: false),
                    PlantationsTypeId = table.Column<int>(nullable: false),
                    StateOfCSR15PlantationsTypeId = table.Column<int>(nullable: true),
                    StateOfCSR15_1 = table.Column<int>(nullable: true),
                    StateOfCSR15_2 = table.Column<int>(nullable: true),
                    StateOfCSR15_3 = table.Column<int>(nullable: true),
                    StateOfCSR15_4 = table.Column<int>(nullable: true),
                    StateOfCSR15_5 = table.Column<int>(nullable: true),
                    Quantity = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreesByObjectTableOfTheBreedStateList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreesByObjectTableOfTheBreedStateList_GreemPlantsPassport_G~",
                        column: x => x.GreemPlantsPassportId,
                        principalTable: "GreemPlantsPassport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreesByObjectTableOfTheBreedStateList_PlantationsType_Plant~",
                        column: x => x.PlantationsTypeId,
                        principalTable: "PlantationsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreesByObjectTableOfTheBreedStateList_PlantationsType_State~",
                        column: x => x.StateOfCSR15PlantationsTypeId,
                        principalTable: "PlantationsType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreesByObjectTableOfTheBreedStateList_GreemPlantsPassportId",
                table: "TreesByObjectTableOfTheBreedStateList",
                column: "GreemPlantsPassportId");

            migrationBuilder.CreateIndex(
                name: "IX_TreesByObjectTableOfTheBreedStateList_PlantationsTypeId",
                table: "TreesByObjectTableOfTheBreedStateList",
                column: "PlantationsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TreesByObjectTableOfTheBreedStateList_StateOfCSR15Plantatio~",
                table: "TreesByObjectTableOfTheBreedStateList",
                column: "StateOfCSR15PlantationsTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreesByObjectTableOfTheBreedStateList");
        }
    }
}
