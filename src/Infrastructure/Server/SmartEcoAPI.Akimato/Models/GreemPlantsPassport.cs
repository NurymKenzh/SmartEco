using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Akimato.Models
{
    public class GreemPlantsPassport
    {
        public int Id { get; set; }

        public string GreenObject { get; set; }

        public int KATOId { get; set; }
        public KATO KATO { get; set; }

        public string NameOfPowersAttributed { get; set; }

        public string NameOfRegistrationObject { get; set; }

        public string LegalEntityUse { get; set; }

        public string AccountNumber { get; set; }

        public string NameAndLocation { get; set; }

        public string PresenceOfHistoricalObject { get; set; }

        public decimal? GreenTotalAreaGa { get; set; }

        public decimal? Lawns { get; set; }

        public decimal? Flowerbeds { get; set; }

        public decimal? TracksAndPlatforms { get; set; }

        public int? Tree { get; set; }

        public int? Shrubs { get; set; }

        public int? SofasAndBenches { get; set; }

        public int? Urns { get; set; }

        public int? EquippedPlaygrounds { get; set; }

        public int? EquippedSportsgrounds { get; set; }

        public int? Monument { get; set; }

        public int? Toilets { get; set; }

        public int? OutdoorLighting { get; set; }

        public int? Billboards { get; set; }

        public int? OtherCapitalStructures { get; set; }

        public decimal? GreenTotalArea { get; set; }

        public decimal? AreaUnderGreenery { get; set; }

        public decimal? AreaUnderLawn { get; set; }

        public decimal? AreaUnderGroundlawn { get; set; }

        public decimal? AreaUnderOrdinarylawn { get; set; }

        public decimal? AreaUnderMeadowlawn { get; set; }

        public decimal? AreaUnderTrees { get; set; }

        public decimal? AreaUnderShrubs { get; set; }

        public decimal? AreaUndeFlowerbeds { get; set; }

        public decimal? AreaUndeTracksAndPlatforms { get; set; }

        public decimal? Asphalted { get; set; }

        public decimal? PavingBlocks { get; set; }

        public decimal? LengthOfTrays { get; set; }

        public int? AmountConiferousTrees { get; set; }

        public int? ListOfTreesConiferous { get; set; }

        public int? Upto10yearsConiferous { get; set; }

        public int? Betwen10_20yearsConiferous { get; set; }

        public int? Over10yearsConiferous { get; set; }

        public int? AmountDeciduousTrees { get; set; }

        public int? ListOfTreesDeciduous { get; set; }

        public int? Upto10yearsDeciduous { get; set; }

        public int? Betwen10_20yearsDeciduous { get; set; }

        public int? Over10yearsDeciduous { get; set; }

        public int? AmountFormedTrees { get; set; }

        public int? TotallAmountShrubs { get; set; }

        public int? AmountShrubs { get; set; }

        public int? LengthOfHedges { get; set; }

        public int? AmountEquippedPlaygrounds { get; set; }

        public int? AmountEquippedSportsgrounds { get; set; }

        public int? AmountSofasAndBenches { get; set; }

        public int? AmountBenches { get; set; }

        public int? AmountSofas { get; set; }

        public int? AmountArbours { get; set; }

        public int? AmountOutdoorLighting { get; set; }

        public int? AmountToilets { get; set; }

        public int? AmountMonument { get; set; }
        
        public int? AmountBillboards { get; set; }
        
        public int? ListOfTreesByObjectBreedsCondition { get; set; }
        
        public int? ListOfTreesByObjectEconomicMeasures { get; set; }
        
        public string PassportGeneralInformation { get; set; }

        public decimal NorthLatitude { get; set; }

        public decimal EastLongitude { get; set; }
    }
}
