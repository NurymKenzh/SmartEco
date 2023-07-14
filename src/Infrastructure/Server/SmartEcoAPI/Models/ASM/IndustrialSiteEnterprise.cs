using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    [Table(nameof(IndSiteEnterprise), Schema = SchemaType.Asm)]
    public class IndSiteEnterprise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EnterpriseId { get; set; }
        public Enterprise Enterprise { get; set; }
        public int MinSizeSanitaryZone { get; set; }
    }
}