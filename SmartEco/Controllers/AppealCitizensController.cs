using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartEco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartEco.Controllers
{
    public class AppealCitizensController : Controller
    {
        private readonly HttpApiClientController _HttpApiClient;

        public AppealCitizensController(HttpApiClientController HttpApiClient)
        {
            _HttpApiClient = HttpApiClient;
        }

        // GET: AppealCitizens
        public async Task<IActionResult> Index()
        {
            PersonQuestions personQuestions = new PersonQuestions();

            string url = "api/AppealCitizens/GetQuestionsAndAnswers",
                route = "";
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            if (response.IsSuccessStatusCode)
            {
                personQuestions = await response.Content.ReadAsAsync<PersonQuestions>();
            }
            ViewBag.PersonQuestions = personQuestions;

            return View();
        }

        //[HttpPost]
        //public async Task<ActionResult> GetQuestion(int id)
        //{
        //    Question question = new Question();
        //    string url = "api/AppealCitizens/GetQuestion",
        //        route = "";
        //    route += string.IsNullOrEmpty(route) ? "?" : "&";
        //    route += $"id={id.ToString()}";
        //    HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        question = await response.Content.ReadAsAsync<Question>();
        //    }

        //    return View(question);
        //}

        // GET: AppealCitizens/AskQuestion
        public async Task<IActionResult> AskQuestion()
        {
            return View();
        }

        // POST: AppealCitizens/AskQuestion
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion([Bind("Id,Name,Text")] Question question)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/AppealCitizens/PostQuestion", question);

                string OutputViewText = await response.Content.ReadAsStringAsync();
                OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                    {
                        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                    }
                    return View(question);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(question);
        }

        // GET: AppealCitizens/ToAnswer
        public async Task<IActionResult> ToAnswer(int questionId)
        {
            Question question = new Question();
            string url = $"api/AppealCitizens/GetQuestion/{questionId}",
                route = "";
            //route += string.IsNullOrEmpty(route) ? "?" : "&";
            //route += $"id={questionId.ToString()}";
            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + route);
            if (response.IsSuccessStatusCode)
            {
                question = await response.Content.ReadAsAsync<Question>();
            }

            ViewBag.Question = question;

            return View();
        }

        // POST: AppealCitizens/ToAnswer
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToAnswer([Bind("Id,Name,Text,QuestionId")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await _HttpApiClient.PostAsJsonAsync(
                    "api/AppealCitizens/PostAnswer", answer);

                string OutputViewText = await response.Content.ReadAsStringAsync();
                OutputViewText = OutputViewText.Replace("<br>", Environment.NewLine);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    dynamic errors = JsonConvert.DeserializeObject<dynamic>(OutputViewText);
                    foreach (Newtonsoft.Json.Linq.JProperty property in errors.Children())
                    {
                        ModelState.AddModelError(property.Name, property.Value[0].ToString());
                    }
                    return View(answer);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(answer);
        }

        // GET: AppealCitizens/DeleteQuestion/5
        public async Task<IActionResult> DeleteQuestion(int? id)
        {
            ViewBag.QuestionId = id;
            return View();
        }

        // POST: AppealCitizens/DeleteQuestion/5
        [HttpPost, ActionName("DeleteQuestion")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionConfirmed(int id)
        {
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/AppealCitizens/DeleteQuestion/{id}");
            return RedirectToAction(nameof(Index));
        }

        // GET: AppealCitizens/DeleteAnswer/5
        public async Task<IActionResult> DeleteAnswer(int? id)
        {
            ViewBag.AnswerId = id;
            return View();
        }

        // POST: AppealCitizens/DeleteAnswer/5
        [HttpPost, ActionName("DeleteAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnswerConfirmed(int id)
        {
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/AppealCitizens/DeleteAnswer/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
