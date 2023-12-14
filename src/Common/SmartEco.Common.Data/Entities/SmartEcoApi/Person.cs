using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEco.Common.Data.Entities.SmartEcoApi
{
    public class Person
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        [NotMapped]
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
    }
}
