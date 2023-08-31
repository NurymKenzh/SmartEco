using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    [Table(nameof(Enterprise), Schema = SchemaType.Asm)]
    public class Enterprise
    {
        public int Id { get; set; }
        public string Bin { get; set; }
        public string Name { get; set; }

        public int? EnterpriseTypeId { get; set; }
        public EnterpriseType EnterpriseType { get; set; }

        public string Contacts { get; set; }
        public KatoEnterprise Kato { get; set; }
    }

    [Table(nameof(KatoEnterprise), Schema = SchemaType.Asm)]
    public class KatoEnterprise
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }

        public int EnterpriseId { get; set; }
        public Enterprise Enterprise { get; set; }
    }
}