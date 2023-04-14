using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class MonitoringPost_20190314_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitoringPost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Number = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NorthLatitude = table.Column<decimal>(nullable: false),
                    EastLongitude = table.Column<decimal>(nullable: false),
                    AdditionalInformation = table.Column<string>(nullable: true),
                    DataProviderId = table.Column<int>(nullable: false),
                    PollutionEnvironmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringPost_DataProvider_DataProviderId",
                        column: x => x.DataProviderId,
                        principalTable: "DataProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonitoringPost_PollutionEnvironment_PollutionEnvironmentId",
                        column: x => x.PollutionEnvironmentId,
                        principalTable: "PollutionEnvironment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringPost_DataProviderId",
                table: "MonitoringPost",
                column: "DataProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringPost_PollutionEnvironmentId",
                table: "MonitoringPost",
                column: "PollutionEnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoringPost");
        }
    }
}
