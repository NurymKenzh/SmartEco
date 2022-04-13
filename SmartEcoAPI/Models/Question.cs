using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsResolved { get; set; }

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
