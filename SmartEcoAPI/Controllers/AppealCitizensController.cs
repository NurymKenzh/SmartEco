using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        /// <param name="PageSize">
        /// Все возвращаемые данные разделены на блоки (страницы). Данный параметр задает размер блока.
        /// </param>
        /// <param name="PageNumber">
        /// Номер возвращаемого блока.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetQuestionsAndAnswers")]
        [Authorize]
        public async Task<ActionResult<PersonQuestions>> GetQuestionsAndAnswers(
            int? PageSize,
            int? PageNumber)
        {
            var questions = _context.Question.Include(q => q.Person).OrderByDescending(q => q.DateTime);
            var answers = _context.Answer.Include(a => a.Question);
            var person = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name);

            if (PageSize != null && PageNumber != null)
            {
                questions = (IOrderedQueryable<Question>)questions.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            var personQuestions = new PersonQuestions();
            try
            {
                var questionAndAnswers = questions
                    .GroupJoin(
                    answers,
                    question => question,
                    answer => answer.Question,
                    (q, ansCollection) => new QuestionAndAnswers
                    {
                        Question = q,
                        Answers = ansCollection.OrderBy(a => a.DateTime).ToList()
                    });

                return new PersonQuestions()
                {
                    QuestionAndAnswers = await questionAndAnswers.ToListAsync(),
                    Person = person
                };
            }
            catch (Exception ex)
            {
                return new PersonQuestions();
            }
        }

        // GET: api/AppealCitizens/5
        /// <summary>
        /// Получение детальной информации отдельного вопроса (работает только для авторизованных пользователей).
        /// </summary>
        /// <param name="id">
        /// Идентификационный номер вопроса (целочисленное значение).
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetQuestion/{id}")]
        [Authorize]
        public async Task<ActionResult<QuestionAndAnswers>> GetQuestion(int id)
        {
            var question = await _context.Question.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            var answer = _context.Answer
                .AsNoTracking()
                .Where(a => a.QuestionId == id)
                .SingleOrDefault();

            return new QuestionAndAnswers() 
            { 
                Question = question, 
                Answers = new List<Answer>() 
                { 
                    answer 
                } 
            };
            //return question;
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
                var isAnswered = _context.Answer.Any(a => a.QuestionId == answer.QuestionId);
                if (isAnswered)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
            }
            catch
            {
                return BadRequest();
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    answer.PersonId = _context.Person.FirstOrDefault(p => p.Email == User.Identity.Name).Id;
                    answer.DateTime = DateTime.Now;
                    _context.Answer.Add(answer);
                    await _context.SaveChangesAsync();

                    var question = await _context.Question.FindAsync(answer.QuestionId);
                    question.IsResolved = true;
                    _context.Question.Update(question);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest();
                }
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
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var answer = await _context.Answer.FindAsync(id);
                    if (answer == null)
                    {
                        return NotFound();
                    }

                    var question = await _context.Question.FindAsync(answer.QuestionId);

                    _context.Answer.Remove(answer);
                    await _context.SaveChangesAsync();

                    question.IsResolved = false;
                    _context.Question.Update(question);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return answer;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest();
                }
            }
        }

        // GET: api/Projects/Count
        [HttpGet("Count")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestionsCount()
        {
            var questions = _context.Question
                .Where(m => true);

            int count = await questions.CountAsync();

            return Ok(count);
        }
    }
}
