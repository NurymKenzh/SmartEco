namespace SmartEco.Models.ASM.Responses
{
    public class AirPollutinSourceLastNumberResponse
    {
        public AirPollutinSourceLastNumberResponse(int number, int count)
        {
            Number = number;
            Count = count;
        }

        public int Number { get; set; }
        public int Count { get; set; }
    }
}
