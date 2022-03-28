using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
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
        public async Task<IActionResult> Index(
            int? PageSize,
            int? PageNumber)
        {
            PersonQuestions personQuestions = new PersonQuestions();

            string url = "api/AppealCitizens",
                route = "",
                routeCount = "";

            IConfigurationSection pageSizeListSection = Startup.Configuration.GetSection("PageSizeList");
            var pageSizeList = pageSizeListSection.AsEnumerable().Where(p => p.Value != null);
            ViewBag.PageSizeList = new SelectList(pageSizeList.OrderBy(p => p.Key)
                .Select(p =>
                {
                    return new KeyValuePair<string, string>(p.Value ?? "0", p.Value);
                }), "Key", "Value");
            if (PageSize == null)
            {
                PageSize = Convert.ToInt32(pageSizeList.Min(p => p.Value));
            }
            if (PageSize == 0)
            {
                PageSize = null;
            }
            if (PageSize != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PageSize={PageSize.ToString()}";
                if (PageNumber == null)
                {
                    PageNumber = 1;
                }
            }
            if (PageNumber != null)
            {
                route += string.IsNullOrEmpty(route) ? "?" : "&";
                route += $"PageNumber={PageNumber.ToString()}";
            }

            HttpResponseMessage response = await _HttpApiClient.GetAsync(url + "/GetQuestionsAndAnswers" + route),
                responseCount = await _HttpApiClient.GetAsync(url + "/Count" + routeCount);
            if (response.IsSuccessStatusCode)
            {
                personQuestions = await response.Content.ReadAsAsync<PersonQuestions>();
            }
            int questionsCount = 0;
            if (responseCount.IsSuccessStatusCode)
            {
                questionsCount = await responseCount.Content.ReadAsAsync<int>();
            }

            ViewBag.PersonQuestions = personQuestions;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber != null ? PageNumber : 1;
            ViewBag.TotalPages = PageSize != null ? (int)Math.Ceiling(questionsCount / (decimal)PageSize) : 1;
            ViewBag.StartPage = PageNumber - 5;
            ViewBag.EndPage = PageNumber + 4;
            if (ViewBag.StartPage <= 0)
            {
                ViewBag.EndPage -= (ViewBag.StartPage - 1);
                ViewBag.StartPage = 1;
            }
            if (ViewBag.EndPage > ViewBag.TotalPages)
            {
                ViewBag.EndPage = ViewBag.TotalPages;
                if (ViewBag.EndPage > 10)
                {
                    ViewBag.StartPage = ViewBag.EndPage - 9;
                }
            }

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
        public async Task<IActionResult> AskQuestion(
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

            return View();
        }

        // POST: AppealCitizens/AskQuestion
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion([Bind("Id,Name,Text")] Question question,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

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

                return RedirectToAction(nameof(Index),
                    new
                    {
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber
                    });
            }

            return View(question);
        }

        // GET: AppealCitizens/ToAnswer
        public async Task<IActionResult> ToAnswer(int questionId,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

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
        public async Task<IActionResult> ToAnswer([Bind("Id,Name,Text,QuestionId")] Answer answer,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

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

                return RedirectToAction(nameof(Index),
                    new
                    {
                        PageSize = ViewBag.PageSize,
                        PageNumber = ViewBag.PageNumber
                    });
            }

            return View(answer);
        }

        // GET: AppealCitizens/DeleteQuestion/5
        public async Task<IActionResult> DeleteQuestion(int? id,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.QuestionId = id;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

            return View();
        }

        // POST: AppealCitizens/DeleteQuestion/5
        [HttpPost, ActionName("DeleteQuestion")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionConfirmed(int id,
            int? PageSize,
            int? PageNumber)
        {
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/AppealCitizens/DeleteQuestion/{id}");
            return RedirectToAction(nameof(Index),
                new
                {
                    PageSize = ViewBag.PageSize,
                    PageNumber = ViewBag.PageNumber
                });
        }

        // GET: AppealCitizens/DeleteAnswer/5
        public async Task<IActionResult> DeleteAnswer(int? id,
            int? PageSize,
            int? PageNumber)
        {
            ViewBag.AnswerId = id;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = PageNumber;

            return View();
        }

        // POST: AppealCitizens/DeleteAnswer/5
        [HttpPost, ActionName("DeleteAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnswerConfirmed(int id,
            int? PageSize,
            int? PageNumber)
        {
            HttpResponseMessage response = await _HttpApiClient.DeleteAsync(
                $"api/AppealCitizens/DeleteAnswer/{id}");
            return RedirectToAction(nameof(Index),
                new
                {
                    PageSize = ViewBag.PageSize,
                    PageNumber = ViewBag.PageNumber
                });
        }
    }
}
