using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class AirPollutionSourceInfo
    {
        public int SourceId { get; set; }
        public AirPollutionSource Source { get; set; }

        public string Coordinate { get; set; }
        public int TerrainCoefficient { get; set; }
        public bool IsCalculateByGas { get; set; }
        public bool IsVerticalDeviation { get; set; }
        public decimal AngleDeflection { get; set; }
        public decimal AngleRotation { get; set; }
        public bool IsCovered { get; set; }
        public bool IsSignFlare { get; set; }
        public decimal Hight { get; set; }
        public decimal Diameter { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public int? RelationBackgroundId { get; set; }
        public RelationBackground RelationBackground { get; set; }
    }

    public class RelationBackground
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
