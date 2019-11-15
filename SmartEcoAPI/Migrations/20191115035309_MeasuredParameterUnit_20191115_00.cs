using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class MeasuredParameterUnit_20191115_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeasuredParameterUnitId",
                table: "MeasuredParameter",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MeasuredParameterUnit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true),
                    NameEN = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredParameterUnit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredParameter_MeasuredParameterUnitId",
                table: "MeasuredParameter",
                column: "MeasuredParameterUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredParameter_MeasuredParameterUnit_MeasuredParameterUn~",
                table: "MeasuredParameter",
                column: "MeasuredParameterUnitId",
                principalTable: "MeasuredParameterUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredParameter_MeasuredParameterUnit_MeasuredParameterUn~",
                table: "MeasuredParameter");

            migrationBuilder.DropTable(
                name: "MeasuredParameterUnit");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredParameter_MeasuredParameterUnitId",
                table: "MeasuredParameter");

            migrationBuilder.DropColumn(
                name: "MeasuredParameterUnitId",
                table: "MeasuredParameter");
        }
    }
}
