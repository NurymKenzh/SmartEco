using SmartEco.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEco.Common.Data.Entities
{
    public class Person
    {
        public long Id { get; set; }
        public required string Email { get; set; }

        [NotMapped]
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public virtual int? RoleId
        {
            get =>(int?)this.Role;
            set => Role = (Role?)value;
        }
        [EnumDataType(typeof(Role))]
        public Role? Role { get; set; }
    }
}
