using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_CreateAPI.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Project_CallAPI.Controllers
{
    public class CourseController : Controller
    {
        private readonly string link = "http://localhost:5091/api/";
        HttpClient _client;

        public CourseController()
        {
            _client = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            string lecturer = HttpContext.Session.GetString("lecturer");
            if(lecturer != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                List<Course> listCourses = new List<Course>();
                User u = JsonConvert.DeserializeObject<User>(lecturer);
                if(u  != null)
                {
                    //Load courses that created by lecturer
                    string odataQuery = "?$filter=LecturerId eq " + u.UserId +"&$expand=Lecturer,Assignments,CourseEnrollments,Quizzes";
                    HttpResponseMessage response = await _client.GetAsync(link + "Course" + odataQuery);
                    if(response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        listCourses = JsonConvert.DeserializeObject<List<Course>>(data);
                        return View(listCourses);
                    }
                }
            }
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<IActionResult> Add(string title, string description)
        {
            string lecturer = HttpContext.Session.GetString("lecturer");
            if (lecturer != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                User u = JsonConvert.DeserializeObject<User>(lecturer);
                if (u != null)
                {
                    var course = new Course()
                    {
                        Title = title,
                        Description = description,
                        LecturerId = u.UserId
                    };
                    HttpResponseMessage response = await _client.PostAsJsonAsync(link + "Course", course);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");
                }
            }
            return Redirect("/Home");
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string title, string description)
        {
            string lecturer = HttpContext.Session.GetString("lecturer");
            if (lecturer != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                var updatedCourse = new Course
                {
                    Title = title,
                    Description = description
                };
                HttpResponseMessage response = await _client.PutAsJsonAsync(link + "Course/" + id, updatedCourse);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            return Redirect("/Home");
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                Course c = new Course();
                string odataQuery = "?$expand=Lecturer,Assignments,CourseEnrollments,Quizzes($expand=QuizAttendances,Questions)";
                HttpResponseMessage response = await _client.GetAsync(link + "Course/" + id + odataQuery);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    c = JsonConvert.DeserializeObject<Course>(data);

                    string user = HttpContext.Session.GetString("user");
                    User u = JsonConvert.DeserializeObject<User>(user);
                    //Only allow student to see detail of the course if they enrolled
                    if (c.CourseEnrollments.Any(enrollment => enrollment.StudentId == u.UserId) || HttpContext.Session.GetString("lecturer") != null)
                    {
                        return View(c);
                    }
                    return RedirectToAction("Index");
                }
            }
            return Redirect("/Login");
        }

        public async Task<IActionResult> Enroll(int id)
        {
            if(HttpContext.Session.GetString("lecturer") == null && HttpContext.Session.GetString("user") != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                string user = HttpContext.Session.GetString("user");
                User u = new User();
                u = JsonConvert.DeserializeObject<User>(user);
                var enrollment = new CourseEnrollment()
                {
                    CourseId = id,
                    StudentId = u.UserId
                };
                HttpResponseMessage response = await _client.PostAsJsonAsync(link + "Course/" + id + "/enroll?studentId=" + u.UserId, enrollment);
                if(response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Unenroll(int id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            if (HttpContext.Session.GetString("lecturer") == null && HttpContext.Session.GetString("user") != null)
            {
                string user = HttpContext.Session.GetString("user");
                User u = new User();
                u = JsonConvert.DeserializeObject<User>(user);
                var enrollment = new CourseEnrollment()
                {
                    CourseId = id,
                    StudentId = u.UserId
                };
                HttpResponseMessage response = await _client.PostAsJsonAsync(link + "Course/" + id + "/unenroll?studentId=" + u.UserId, enrollment);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index1()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            string user = HttpContext.Session.GetString("user");
            if (user != null)
            {
                List<CourseEnrollment> listEnrollments = new List<CourseEnrollment>();
                User u = new User();
                u = JsonConvert.DeserializeObject<User>(user);
                if(u != null)
                {
                    string odataQuery = "?$filter=StudentId eq "+ u.UserId +"&$expand=Student,Course";
                    HttpResponseMessage response = await _client.GetAsync(link + "Course/enrollments" + odataQuery);
                    if (response.IsSuccessStatusCode)
                    { 
                        string data = await response.Content.ReadAsStringAsync();
                        listEnrollments = JsonConvert.DeserializeObject<List<CourseEnrollment>>(data);
                        return View(listEnrollments);
                    }
                }
            }
            return Redirect("/Home");
        }

        public async Task<IActionResult> List(int id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            string lecturer = HttpContext.Session.GetString("lecturer");
            if(lecturer != null)
            {
                List<CourseEnrollment> listEnrollments = new List<CourseEnrollment>();
                string odataQuery = "?$filter=CourseId eq " + id + "&$expand=Student,Course";
                HttpResponseMessage response = await _client.GetAsync(link + "Course/enrollments" + odataQuery);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    listEnrollments = JsonConvert.DeserializeObject<List<CourseEnrollment>>(data);
                    return View(listEnrollments);
                }
            }
            return Redirect("/Home");
        }

        public async Task<IActionResult> Delete(int id)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.DeleteAsync(link + "Course/" + id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
