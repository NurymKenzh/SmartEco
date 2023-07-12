using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    [Table(nameof(EnterpriseType), Schema = SchemaType.Asm)]
    public class EnterpriseType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}