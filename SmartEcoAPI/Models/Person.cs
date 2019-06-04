using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [NotMapped]
        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
