using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class Enterprise_20230714_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Enterprise_KatoId",
                schema: "asm",
                table: "Enterprise",
                column: "KatoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enterprise_KATO_KatoId",
                schema: "asm",
                table: "Enterprise",
                column: "KatoId",
                principalTable: "KATO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enterprise_KATO_KatoId",
                schema: "asm",
                table: "Enterprise");

            migrationBuilder.DropIndex(
                name: "IX_Enterprise_KatoId",
                schema: "asm",
                table: "Enterprise");
        }
    }
}
