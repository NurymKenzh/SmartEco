using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoAPI.Akimato.Migrations
{
    public partial class GreemPlantsPassport_20201029_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GreemPlantsPassport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GreenObject = table.Column<string>(nullable: true),
                    KATOId = table.Column<int>(nullable: false),
                    NameOfPowersAttributed = table.Column<string>(nullable: true),
                    NameOfRegistrationObject = table.Column<string>(nullable: true),
                    LegalEntityUse = table.Column<string>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true),
                    NameAndLocation = table.Column<string>(nullable: true),
                    PresenceOfHistoricalObject = table.Column<string>(nullable: true),
                    GreenTotalAreaGa = table.Column<decimal>(nullable: true),
                    Lawns = table.Column<decimal>(nullable: true),
                    Flowerbeds = table.Column<decimal>(nullable: true),
                    TracksAndPlatforms = table.Column<decimal>(nullable: true),
                    Tree = table.Column<int>(nullable: true),
                    Shrubs = table.Column<int>(nullable: true),
                    SofasAndBenches = table.Column<int>(nullable: true),
                    Urns = table.Column<int>(nullable: true),
                    EquippedPlaygrounds = table.Column<int>(nullable: true),
                    EquippedSportsgrounds = table.Column<int>(nullable: true),
                    Monument = table.Column<int>(nullable: true),
                    Toilets = table.Column<int>(nullable: true),
                    OutdoorLighting = table.Column<int>(nullable: true),
                    Billboards = table.Column<int>(nullable: true),
                    OtherCapitalStructures = table.Column<int>(nullable: true),
                    GreenTotalArea = table.Column<decimal>(nullable: true),
                    AreaUnderGreenery = table.Column<decimal>(nullable: true),
                    AreaUnderLawn = table.Column<decimal>(nullable: true),
                    AreaUnderGroundlawn = table.Column<decimal>(nullable: true),
                    AreaUnderOrdinarylawn = table.Column<decimal>(nullable: true),
                    AreaUnderMeadowlawn = table.Column<decimal>(nullable: true),
                    AreaUnderTrees = table.Column<decimal>(nullable: true),
                    AreaUnderShrubs = table.Column<decimal>(nullable: true),
                    AreaUndeFlowerbeds = table.Column<decimal>(nullable: true),
                    AreaUndeTracksAndPlatforms = table.Column<decimal>(nullable: true),
                    Asphalted = table.Column<decimal>(nullable: true),
                    PavingBlocks = table.Column<decimal>(nullable: true),
                    LengthOfTrays = table.Column<decimal>(nullable: true),
                    AmountConiferousTrees = table.Column<int>(nullable: true),
                    ListOfTreesConiferous = table.Column<int>(nullable: true),
                    Upto10yearsConiferous = table.Column<int>(nullable: true),
                    Betwen10_20yearsConiferous = table.Column<int>(nullable: true),
                    Over10yearsConiferous = table.Column<int>(nullable: true),
                    AmountDeciduousTrees = table.Column<int>(nullable: true),
                    ListOfTreesDeciduous = table.Column<int>(nullable: true),
                    Upto10yearsDeciduous = table.Column<int>(nullable: true),
                    Betwen10_20yearsDeciduous = table.Column<int>(nullable: true),
                    Over10yearsDeciduous = table.Column<int>(nullable: true),
                    AmountFormedTrees = table.Column<int>(nullable: true),
                    TotallAmountShrubs = table.Column<int>(nullable: true),
                    AmountShrubs = table.Column<int>(nullable: true),
                    LengthOfHedges = table.Column<int>(nullable: true),
                    AmountEquippedPlaygrounds = table.Column<int>(nullable: true),
                    AmountEquippedSportsgrounds = table.Column<int>(nullable: true),
                    AmountSofasAndBenches = table.Column<int>(nullable: true),
                    AmountBenches = table.Column<int>(nullable: true),
                    AmountSofas = table.Column<int>(nullable: true),
                    AmountArbours = table.Column<int>(nullable: true),
                    AmountOutdoorLighting = table.Column<int>(nullable: true),
                    AmountToilets = table.Column<int>(nullable: true),
                    AmountMonument = table.Column<int>(nullable: true),
                    AmountBillboards = table.Column<int>(nullable: true),
                    ListOfTreesByObjectBreedsCondition = table.Column<int>(nullable: true),
                    ListOfTreesByObjectEconomicMeasures = table.Column<int>(nullable: true),
                    PassportGeneralInformation = table.Column<string>(nullable: true),
                    NorthLatitude = table.Column<decimal>(nullable: false),
                    EastLongitude = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GreemPlantsPassport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GreemPlantsPassport_KATO_KATOId",
                        column: x => x.KATOId,
                        principalTable: "KATO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GreemPlantsPassport_KATOId",
                table: "GreemPlantsPassport",
                column: "KATOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GreemPlantsPassport");
        }
    }
}
