namespace SmartEco.Common.Data.Entities.SmartEcoApi
{
    public class MeasuredParameter
    {
        public int Id { get; set; }
        public int? MeasuredParameterUnitId { get; set; }
        public MeasuredParameterUnit? MeasuredParameterUnit { get; set; }
        public string? NameKK { get; set; }
        public string? NameRU { get; set; }
        public string? NameEN { get; set; }
        public string? NameShortKK { get; set; }
        public string? NameShortRU { get; set; }
        public string? NameShortEN { get; set; }
        public int? EcomonCode { get; set; }
        public string? OceanusCode { get; set; }
        public string? KazhydrometCode { get; set; }
        public decimal? MPCDailyAverage { get; set; } // maximum permissible concentration
        public decimal? MPCMaxSingle { get; set; }
        public int? PollutionEnvironmentId { get; set; }
        public PollutionEnvironment? PollutionEnvironment { get; set; }
    }
}
