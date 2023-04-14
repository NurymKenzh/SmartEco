using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class Layer_20190313_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Layer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GeoServerWorkspace = table.Column<string>(nullable: true),
                    GeoServerName = table.Column<string>(nullable: true),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true),
                    NameEN = table.Column<string>(nullable: true),
                    PollutionEnvironmentId = table.Column<int>(nullable: true),
                    MeasuredParameterId = table.Column<int>(nullable: true),
                    KATOId = table.Column<int>(nullable: true),
                    Season = table.Column<int>(nullable: false),
                    Hour = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Layer_KATO_KATOId",
                        column: x => x.KATOId,
                        principalTable: "KATO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Layer_MeasuredParameter_MeasuredParameterId",
                        column: x => x.MeasuredParameterId,
                        principalTable: "MeasuredParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Layer_PollutionEnvironment_PollutionEnvironmentId",
                        column: x => x.PollutionEnvironmentId,
                        principalTable: "PollutionEnvironment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Layer_KATOId",
                table: "Layer",
                column: "KATOId");

            migrationBuilder.CreateIndex(
                name: "IX_Layer_MeasuredParameterId",
                table: "Layer",
                column: "MeasuredParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_Layer_PollutionEnvironmentId",
                table: "Layer",
                column: "PollutionEnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Layer");
        }
    }
}
