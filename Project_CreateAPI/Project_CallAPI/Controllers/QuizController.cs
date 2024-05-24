using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_CreateAPI.Models;
using System.Net.Http.Headers;

namespace Project_CallAPI.Controllers
{
    public class QuizController : Controller
    {
        private readonly string link = "http://localhost:5091/api/";
        HttpClient _client;

        public QuizController()
        {
            _client = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Quiz dto)
        {
            string lecturer = HttpContext.Session.GetString("lecturer");
            if (lecturer != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                User u = JsonConvert.DeserializeObject<User>(lecturer);
                if (u != null)
                {
                    var quiz = new Quiz()
                    {
                        CourseId = dto.CourseId,
                        Title = dto.Title,
                    };
                    HttpResponseMessage response = await _client.PostAsJsonAsync(link + "Quiz", quiz);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Detail", "Course", new { id = quiz.CourseId });
                    }
                    return RedirectToAction("Detail", "Course", new { id = quiz.CourseId });
                }
                
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(Quiz dto)
        {
            var quiz = new Quiz()
            {
                QuizId = dto.QuizId,
                CourseId = dto.CourseId,
                Title = dto.Title,
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.PutAsJsonAsync(link + "Quiz/" + dto.QuizId, quiz);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Course", new { id = quiz.CourseId });
            }
            return RedirectToAction("Detail", "Course", new { id = quiz.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (HttpContext.Session.GetString("lecturer") != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                Quiz q = new Quiz();
                string odataQuery = "?$expand=Course,Questions,QuizAttendances";
                HttpResponseMessage response = await _client.GetAsync(link + "Quiz/" + id + odataQuery);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    q = JsonConvert.DeserializeObject<Quiz>(data);
                    return View(q);
                }
            }
            return Redirect("/Home");
        }

        [HttpGet]
        public async Task<IActionResult> Attempt(int id)
        {
            if(HttpContext.Session.GetString("user") != null && HttpContext.Session.GetString("lecturer") == null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                Quiz q = new Quiz();
                string odataQuery = "?$expand=Course,Questions,QuizAttendances";
                HttpResponseMessage response = await _client.GetAsync(link + "Quiz/" + id + odataQuery);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    q = JsonConvert.DeserializeObject<Quiz>(data);
                    return View(q);
                }
            }
            return Redirect("/Home");
        }

        public async Task<IActionResult> List(int id)
        {
            if(HttpContext.Session.GetString("lecturer") != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                Quiz q = new Quiz();
                string odataQuery = "?$expand=Course,Questions,QuizAttendances($expand=Student)";
                HttpResponseMessage response = await _client.GetAsync(link + "Quiz/" + id + odataQuery);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    q = JsonConvert.DeserializeObject<Quiz>(data);
                    return View(q);
                }
            }
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<IActionResult> Attempt(QuizAttendance model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            string user = HttpContext.Session.GetString("user");
            List<Question> questions = new List<Question>();
            string odataQuery = "?$filter=QuizId eq " + model.QuizId +"&$expand=Quiz";
            HttpResponseMessage response = await _client.GetAsync(link + "Question" + odataQuery);
            string data = await response.Content.ReadAsStringAsync();
            questions = JsonConvert.DeserializeObject<List<Question>>(data);
            int score = CalculateScore(questions);
            User u = JsonConvert.DeserializeObject<User>(user);
            QuizAttendance result = new QuizAttendance()
            {
                QuizId = model.QuizId,
                StudentId = u.UserId,
                Score = score
            };
            HttpResponseMessage response1 = await _client.PostAsJsonAsync(link + "QuizResult", result);
            if (response1.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Course", new { id = questions[0].Quiz.CourseId });
            }
            return RedirectToAction("Detail", "Course", new { id = questions[0].Quiz.CourseId });
        }

        private int CalculateScore(List<Question> questions)
        {
            int score = 0;
            foreach(var question in questions)
            {
                string selectedAnswer = Request.Form["question-" + question.QuestionId];
                if(selectedAnswer == question.CorrectOption)
                {
                    score++;
                }
            }
            return score;
        }

        public async Task<IActionResult> DeleteAttempt(int id, int quizId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.DeleteAsync(link + "QuizResult/" + id + "&" + quizId);
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction("List", "Quiz", new { id = quizId });
            }
            return RedirectToAction("List", "Quiz", new { id = quizId });
        }

        public async Task<IActionResult> Delete(int id, int courseId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.DeleteAsync(link + "Quiz/" + id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Course", new { id = courseId });
            }
            return RedirectToAction("Detail", "Course", new { id = courseId });
        }
    }
}
