using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class AActivity_20200813_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Efficiency",
                table: "AActivity",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndPeriod",
                table: "AActivity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AActivity",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartPeriod",
                table: "AActivity",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetValueId",
                table: "AActivity",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AActivity_TargetValueId",
                table: "AActivity",
                column: "TargetValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_AActivity_TargetValue_TargetValueId",
                table: "AActivity",
                column: "TargetValueId",
                principalTable: "TargetValue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AActivity_TargetValue_TargetValueId",
                table: "AActivity");

            migrationBuilder.DropIndex(
                name: "IX_AActivity_TargetValueId",
                table: "AActivity");

            migrationBuilder.DropColumn(
                name: "Efficiency",
                table: "AActivity");

            migrationBuilder.DropColumn(
                name: "EndPeriod",
                table: "AActivity");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AActivity");

            migrationBuilder.DropColumn(
                name: "StartPeriod",
                table: "AActivity");

            migrationBuilder.DropColumn(
                name: "TargetValueId",
                table: "AActivity");
        }
    }
}
