using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEco.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "NameUser")]
        public string Name { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Text")]
        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }
    }
    public class PersonQuestions
    {
        public PersonQuestions()
        {
            QuestionAndAnswers = new List<QuestionAndAnswers>();
        }
        public Person Person { get; set; }
        public List<QuestionAndAnswers> QuestionAndAnswers { get; set; }
    }
    public class QuestionAndAnswers
    {
        public Question Question { get; set; }
        public IEnumerable<Answer> Answers { get; set; }
    }
}
