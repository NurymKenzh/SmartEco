using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class ResultEmission_20240313_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FeatureCollection",
                schema: "asmUprza",
                table: "ResultEmission",
                newName: "RectanglesFeatures");

            migrationBuilder.AddColumn<string>(
                name: "PointsFeatures",
                schema: "asmUprza",
                table: "ResultEmission",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsFeatures",
                schema: "asmUprza",
                table: "ResultEmission");

            migrationBuilder.RenameColumn(
                name: "RectanglesFeatures",
                schema: "asmUprza",
                table: "ResultEmission",
                newName: "FeatureCollection");
        }
    }
}
