using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Models
{
    public class Answer
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public int QuestionId { get; set; }

        public Question Question { get; set; }
    }
}
