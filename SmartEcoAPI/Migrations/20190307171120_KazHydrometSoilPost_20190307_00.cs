using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class KazHydrometSoilPost_20190307_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KazHydrometSoilPostId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "MeasuredData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KazHydrometSoilPost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Number = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    NorthLatitude = table.Column<decimal>(nullable: false),
                    EastLongitude = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KazHydrometSoilPost", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredData_KazHydrometSoilPostId",
                table: "MeasuredData",
                column: "KazHydrometSoilPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredData_KazHydrometSoilPost_KazHydrometSoilPostId",
                table: "MeasuredData",
                column: "KazHydrometSoilPostId",
                principalTable: "KazHydrometSoilPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_KazHydrometSoilPost_KazHydrometSoilPostId",
                table: "MeasuredData");

            migrationBuilder.DropTable(
                name: "KazHydrometSoilPost");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_KazHydrometSoilPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "KazHydrometSoilPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "MeasuredData");
        }
    }
}
