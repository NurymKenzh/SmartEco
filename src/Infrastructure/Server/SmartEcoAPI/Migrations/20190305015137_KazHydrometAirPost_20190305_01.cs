using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoAPI.Migrations
{
    public partial class KazHydrometAirPost_20190305_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdditionalInformationRU",
                table: "KazHydrometAirPost",
                newName: "AdditionalInformation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdditionalInformation",
                table: "KazHydrometAirPost",
                newName: "AdditionalInformationRU");
        }
    }
}
