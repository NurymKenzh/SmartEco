using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WorkshopId { get; set; }
        public Workshop Workshop { get; set; }
    }
}
