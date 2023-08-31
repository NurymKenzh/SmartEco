using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class KatoEnterprise_20230831_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enterprise_KATO_KatoId",
                schema: "asm",
                table: "Enterprise");

            migrationBuilder.DropIndex(
                name: "IX_Enterprise_KatoId",
                schema: "asm",
                table: "Enterprise");

            migrationBuilder.DropColumn(
                name: "KatoId",
                schema: "asm",
                table: "Enterprise");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KatoId",
                schema: "asm",
                table: "Enterprise",
                nullable: false,
                defaultValue: 0);

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
    }
}
