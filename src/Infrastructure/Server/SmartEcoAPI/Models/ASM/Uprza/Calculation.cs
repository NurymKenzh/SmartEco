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
    }
}
