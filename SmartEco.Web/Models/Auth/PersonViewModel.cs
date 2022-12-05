using SmartEco.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace SmartEco.Web.Models.Auth
{
    public class PersonAuthViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Resources.Helpers.SharedResources), Name = "Login")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public Role? Role { get; set; }
    }

    public class PersonViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(ResourceType = typeof(Resources.Helpers.SharedResources), Name = "Login")]
        public string Email { get; set; }

        [Display(Name = "Role")]
        public Role? Role { get; set; }
    }
}
