namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class AirPollutant
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
        public string Formula { get; set; }
        public decimal? MpcMaxSingle { get; set; }
        public decimal? MpcAvgDaily { get; set; }
        public decimal? Asel { get; set; } //ОБУВ (Ориентировочно Безопасный Уровень Воздействия/Approximately Safe Exposure Level)

        public int? HazardLevelId { get; set; }
        public HazardLevel HazardLevel { get; set; }

        public string MeasuredUnit { get; set; }
        public string Cas { get; set; } //CAS (Chemical Abstracts Service/Химическая реферативная служба)
        public decimal? MpcMaxSingle2 { get; set; }
        public string SummationGroup { get; set; }
        public string AggregationState { get; set; }
    }
}
