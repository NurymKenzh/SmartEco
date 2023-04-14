using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class Targets_col_ProjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "TargetValue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "TargetTerritory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Target",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "AActivity",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetValue_ProjectId",
                table: "TargetValue",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTerritory_ProjectId",
                table: "TargetTerritory",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Target_ProjectId",
                table: "Target",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_ProjectId",
                table: "Event",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AActivity_ProjectId",
                table: "AActivity",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AActivity_Project_ProjectId",
                table: "AActivity",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Project_ProjectId",
                table: "Event",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Target_Project_ProjectId",
                table: "Target",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTerritory_Project_ProjectId",
                table: "TargetTerritory",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetValue_Project_ProjectId",
                table: "TargetValue",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AActivity_Project_ProjectId",
                table: "AActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Project_ProjectId",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Target_Project_ProjectId",
                table: "Target");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetTerritory_Project_ProjectId",
                table: "TargetTerritory");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetValue_Project_ProjectId",
                table: "TargetValue");

            migrationBuilder.DropIndex(
                name: "IX_TargetValue_ProjectId",
                table: "TargetValue");

            migrationBuilder.DropIndex(
                name: "IX_TargetTerritory_ProjectId",
                table: "TargetTerritory");

            migrationBuilder.DropIndex(
                name: "IX_Target_ProjectId",
                table: "Target");

            migrationBuilder.DropIndex(
                name: "IX_Event_ProjectId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_AActivity_ProjectId",
                table: "AActivity");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TargetValue");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TargetTerritory");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Target");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AActivity");
        }
    }
}
