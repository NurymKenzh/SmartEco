using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppealCitizensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppealCitizensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AppealCitizens
        /// <summary>
        /// Получение всех имеющихся вопросов и ответов на них (работает только для авторизованных пользователей).
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetQuestionsAndAnswers")]
        [Authorize]
        public async Task<ActionResult<PersonQuestions>> GetQuestionsAndAnswers()
        {
            var questions = _context.Question.Include(q => q.Person).OrderByDescending(q => q.DateTime).ToList();
            var answers = _context.Answer.Include(a => a.Question).ToList();
            var person = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name);

            var personQuestions = new PersonQuestions();
            var questionAndAnswers = new List<QuestionAndAnswers>();
            try
            {
                questionAndAnswers = questions
                    .AsEnumerable()
                    .GroupJoin(
                    answers,
                    question => question,
                    answer => answer.Question,
                    (q, ansCollection) => new QuestionAndAnswers
                    {
                        Question = q,
                        Answers = ansCollection.OrderBy(a => a.DateTime).ToList()
                    }).ToList();

                personQuestions.Person = person;
                personQuestions.QuestionAndAnswers = questionAndAnswers;
            }
            catch (Exception ex)
            {

            }
            return personQuestions;
        }

        // GET: api/AppealCitizens/5
        /// <summary>
        /// Получение детальной информации отдельного вопроса (работает только для администратора или модератора).
        /// </summary>
        /// <param name="id">
        /// Идентификационный номер вопроса (целочисленное значение).
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetQuestion/{id}")]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Question.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // POST: api/AppealCitizens
        /// <summary>
        /// Добавление нового вопроса в систему (работает только для авторизованных пользователей).
        /// </summary>
        /// <param name="question">
        /// Объект для создания нового вопроса.
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostQuestion")]
        [Authorize]
        public async Task<ActionResult<Question>> PostQuestion(Question question)
        {
            question.PersonId = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name).Id;
            question.DateTime = DateTime.Now;
            _context.Question.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestion", new { id = question.Id }, question);
        }

        // POST: api/AppealCitizens
        /// <summary>
        /// Добавление нового ответа на вопрос в систему (работает только для администратора или модератора).
        /// </summary>
        /// <param name="answer">
        /// Объект для создания нового ответа.
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostAnswer")]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult<Answer>> PostAnswer(Answer answer)
        {
            try
            {
                answer.PersonId = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name).Id;
                answer.DateTime = DateTime.Now;
                _context.Answer.Add(answer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return answer;
        }

        // DELETE: api/AppealCitizens/5
        /// <summary>
        /// Удаление вопроса и всех связанных ответов из системы (работает только для авторизованных пользователей).
        /// </summary>
        /// <param name="id">
        /// Идентификационный номер вопроса (целочисленное значение).
        /// </param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteQuestion/{id}")]
        [Authorize]
        public async Task<ActionResult<Question>> DeleteQuestion(int id)
        {
            var question = await _context.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Question.Remove(question);
            await _context.SaveChangesAsync();

            return question;
        }

        // DELETE: api/AppealCitizens/5
        /// <summary>
        /// Удаление ответа из системы (работает только для администратора или модератора).
        /// </summary>
        /// <param name="id">
        /// Идентификационный номер ответа (целочисленное значение).
        /// </param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteAnswer/{id}")]
        [Authorize(Roles = "admin, moderator")]
        public async Task<ActionResult<Answer>> DeleteAnswer(int id)
        {
            var answer = await _context.Answer.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            _context.Answer.Remove(answer);
            await _context.SaveChangesAsync();

            return answer;
        }
    }
}
