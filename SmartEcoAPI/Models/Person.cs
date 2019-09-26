using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    /// <summary>
    /// Пользователь.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Id пользователя. Не вводится.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Электронная почта пользователя.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [NotMapped]
        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        public string Password { get; set; }

        /// <summary>
        /// Хэш пароля. Не вводится.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Роль пользователя. Не вводится.
        /// </summary>
        public string Role { get; set; }
    }
}
