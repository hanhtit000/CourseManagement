using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project_CreateAPI.Models;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Project_CallAPI.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly string link = "http://localhost:5091/api/";
        HttpClient _client;

        public AssignmentController()
        {
            _client = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Assignment dto, IFormFile? file)
        {
            var assignment = new Assignment()
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                Display = dto.Display,
                Deadline = dto.Deadline
            };

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(assignment.CourseId.ToString()), "courseId");
            formData.Add(new StringContent(assignment.Title), "title");
            formData.Add(new StringContent(assignment.Description), "description");
            formData.Add(new StringContent(assignment.Display.ToString()), "display");
            formData.Add(new StringContent(assignment.Deadline.ToString()), "deadline");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                formData.Add(fileContent, "file", file.FileName);
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.PostAsync(link + "Assignment", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Course", new { id = assignment.CourseId });
            }
            return RedirectToAction("Detail", "Course", new { id = assignment.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Assignment dto, IFormFile? file)
        {
            var assignment = new Assignment()
            {
                AssignmentId = dto.AssignmentId,
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                Display = dto.Display,
                Deadline = dto.Deadline
            };

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(assignment.AssignmentId.ToString()), "assignmentId");
            formData.Add(new StringContent(assignment.CourseId.ToString()), "courseId");
            formData.Add(new StringContent(assignment.Title), "title");
            formData.Add(new StringContent(assignment.Description), "description");
            formData.Add(new StringContent(assignment.Display.ToString()), "display");
            formData.Add(new StringContent(assignment.Deadline.ToString()), "deadline");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                formData.Add(fileContent, "file", file.FileName);
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.PutAsync(link + "Assignment/" + dto.AssignmentId, formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Course", new { id = assignment.CourseId });
            }
            return RedirectToAction("Detail", "Course", new { id = assignment.CourseId });
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                Assignment a = new Assignment();
                string odataQuery = "?$expand=Course, StudentAssignments";
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                HttpResponseMessage response = await _client.GetAsync(link + "Assignment/" + id + odataQuery);
                if(response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    a = JsonConvert.DeserializeObject<Assignment>(data);
                    return View(a);
                }
            }
            return Redirect("/Home");
        }

        public async Task<IActionResult> List(int id)
        {
            if (HttpContext.Session.GetString("lecturer") != null)
            {
                Assignment a = new Assignment();
                string odataQuery = "?$expand=Course, StudentAssignments($expand=Student)";
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                HttpResponseMessage response = await _client.GetAsync(link + "Assignment/" + id + odataQuery);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    a = JsonConvert.DeserializeObject<Assignment>(data);
                    return View(a);
                }
            }
            return Redirect("/Home");
        }

        public async Task<IActionResult> Delete(int id, int courseId)
        {
            HttpResponseMessage response = await _client.DeleteAsync(link + "Assignment/" + id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Course", new { id = courseId});
            }
            return RedirectToAction("Detail", "Course", new { id = courseId });
        }
    }
}
