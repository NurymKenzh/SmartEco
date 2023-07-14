using SmartEcoAPI.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEcoAPI.Models.ASM
{
    [Table(nameof(Enterprise), Schema = SchemaType.Asm)]
    public class Enterprise
    {
        public int Id { get; set; }
        public long Bin { get; set; }
        public string Name { get; set; }
        public int KatoId { get; set; }
        public KATO Kato { get; set; }
        public int? EnterpriseTypeId { get; set; }
        public EnterpriseType EnterpriseType { get; set; }
        public string Contacts { get; set; }
    }
}