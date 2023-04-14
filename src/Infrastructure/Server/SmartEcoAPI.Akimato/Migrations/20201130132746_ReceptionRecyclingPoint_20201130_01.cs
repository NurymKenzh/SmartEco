using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class ReceptionRecyclingPoint_20201130_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NorthLatitude",
                table: "ReceptionRecyclingPoint",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "EastLongitude",
                table: "ReceptionRecyclingPoint",
                nullable: true,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "NorthLatitude",
                table: "ReceptionRecyclingPoint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EastLongitude",
                table: "ReceptionRecyclingPoint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
