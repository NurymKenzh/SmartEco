using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class Enterprise_20230829_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Bin",
                schema: "asm",
                table: "Enterprise",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Bin",
                schema: "asm",
                table: "Enterprise",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
