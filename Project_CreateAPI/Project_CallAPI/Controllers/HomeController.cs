using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project_CallAPI.Models;
using Project_CreateAPI.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Project_CallAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly string link = "http://localhost:5091/api/";
        HttpClient _client;

        public HomeController()
        {
            _client = new HttpClient();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string lecturer, string keyword)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                List<Course> listCourses = new List<Course>();
                List<User> listLecturer = new List<User>();
                string odataQuery = "";
                if(lecturer!=null) odataQuery= "?$filter= contains(Title, '" + keyword + "') and LecturerId eq " + Int32.Parse(lecturer) +"&$expand=Lecturer,Assignments,CourseEnrollments,Quizzes";
                else odataQuery = "?$filter= contains(Title, '" + keyword + "')&$expand=Lecturer,Assignments,CourseEnrollments,Quizzes";
                HttpResponseMessage response = await _client.GetAsync(link + "Course" + odataQuery);
                string odataQuery2 = "?$filter= Role eq 'Lecturer'";
                HttpResponseMessage response2 = await _client.GetAsync(link + "User" + odataQuery2);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listCourses = JsonConvert.DeserializeObject<List<Course>>(data);
                    string data2 = await response2.Content.ReadAsStringAsync();
                    listLecturer = JsonConvert.DeserializeObject<List<User>>(data2);
                    ViewBag.FilterData = listLecturer;
                    ViewBag.Keyword = keyword;
                    ViewBag.Lecturer = lecturer;
                    return View(listCourses);
                }
            }
            return Redirect("/Login");
        }
    }
}