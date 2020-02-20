using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Migrations
{
    public partial class Target_20200212_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Target",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    PollutionEnvironmentId = table.Column<int>(nullable: false),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true),
                    NameEN = table.Column<string>(nullable: true),
                    TypeOfAchievement = table.Column<bool>(nullable: false),
                    MeasuredParameterUnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Target", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Target_MeasuredParameterUnit_MeasuredParameterUnitId",
                        column: x => x.MeasuredParameterUnitId,
                        principalTable: "MeasuredParameterUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Target_PollutionEnvironment_PollutionEnvironmentId",
                        column: x => x.PollutionEnvironmentId,
                        principalTable: "PollutionEnvironment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Target_MeasuredParameterUnitId",
                table: "Target",
                column: "MeasuredParameterUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Target_PollutionEnvironmentId",
                table: "Target",
                column: "PollutionEnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Target");
        }
    }
}
