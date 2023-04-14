using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class MeasuredData_20190306_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "MeasuredData",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "MeasuredData",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "MaxValueDay",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxValueMonth",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "MeasuredData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "MeasuredData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxValueDay",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "MaxValueMonth",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "MeasuredData");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "MeasuredData");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "MeasuredData",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "MeasuredData",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
