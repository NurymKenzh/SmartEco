namespace SmartEcoAPI.Models.ASM.Uprza
{
    public class Calculation
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int TypeId { get; set; }
        public CalculationType Type { get; set; }

        public int StatusId { get; set; }
        public CalculationStatus Status { get; set; }

        public string KatoCode { get; set; }
        public string KatoName { get; set; }

        #region One-to-one relation
        public CalculationSetting Setting { get; set; }
        public StateCalculation State { get; set; }
        #endregion
    }

    public enum CalculationStatuses
    {
        New = 1,
        Configuration = 2,
        Initiated = 3,
        Error = 4,
        Done = 5
    }
}
