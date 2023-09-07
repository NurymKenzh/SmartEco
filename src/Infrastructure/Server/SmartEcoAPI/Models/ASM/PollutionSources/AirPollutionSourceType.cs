using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM.PollutionSources
{
    [Table(nameof(AirPollutionSourceType), Schema = SchemaType.Asm)]
    public class AirPollutionSourceType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOrganized { get; set; }
    }
}
