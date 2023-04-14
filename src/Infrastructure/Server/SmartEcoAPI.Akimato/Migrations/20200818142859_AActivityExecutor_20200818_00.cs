using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class AActivityExecutor_20200818_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AActivityExecutor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AActivityId = table.Column<int>(nullable: false),
                    ExecutorId = table.Column<int>(nullable: false),
                    Contribution = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AActivityExecutor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AActivityExecutor_AActivity_AActivityId",
                        column: x => x.AActivityId,
                        principalTable: "AActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AActivityExecutor_Executor_ExecutorId",
                        column: x => x.ExecutorId,
                        principalTable: "Executor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AActivityExecutor_AActivityId",
                table: "AActivityExecutor",
                column: "AActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AActivityExecutor_ExecutorId",
                table: "AActivityExecutor",
                column: "ExecutorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AActivityExecutor");
        }
    }
}
