using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    [Table(nameof(Workshop), Schema = SchemaType.Asm)]
    public class Workshop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IndSiteEnterpriseId { get; set; }
        public IndSiteEnterprise IndSiteEnterprise { get; set; }
    }
}
