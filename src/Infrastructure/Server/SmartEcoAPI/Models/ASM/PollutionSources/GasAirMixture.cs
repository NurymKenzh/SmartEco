namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    public class GasAirMixture
    {
        public int OperationModeId { get; set; }
        public OperationMode OperationMode { get; set; }

        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Speed { get; set; }
        public double Volume { get; set; }
        public double Humidity { get; set; }
        public double Density { get; set; }
        public double ThermalPower { get; set; }
        public double PartRadiation { get; set; }
    }
}
