using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class KATO_20190313_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredData_KazHydrometSoilPost_KazHydrometSoilPostId",
                table: "MeasuredData");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredData_KazHydrometSoilPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "KazHydrometSoilPostId",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "MeasuredData");

            migrationBuilder.CreateTable(
                name: "KATO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Code = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    AreaType = table.Column<int>(nullable: false),
                    EgovId = table.Column<int>(nullable: false),
                    ParentEgovId = table.Column<int>(nullable: false),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KATO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PollutionEnvironment",
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
                    table.PrimaryKey("PK_PollutionEnvironment", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KATO");

            migrationBuilder.DropTable(
                name: "PollutionEnvironment");

            migrationBuilder.AddColumn<int>(
                name: "KazHydrometSoilPostId",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "MeasuredData",
                nullable: false,
                defaultValue: 0);

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
    }
}
