using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Text")]
        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
