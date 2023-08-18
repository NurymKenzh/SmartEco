using SmartEcoAPI.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    [Table(nameof(SanZoneEnterpriseBorder), Schema = SchemaType.Asm)]
    public class SanZoneEnterpriseBorder
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal PermissibleConcentration { get; set; }

        public int IndSiteEnterpriseId { get; set; }
        public IndSiteEnterprise IndSiteEnterprise { get; set; }

        public List<string> Coordinates { get; set; }
    }
}
