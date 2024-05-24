using Microsoft.AspNetCore.Mvc;
using Project_CreateAPI.Models;
using System.Net.Http.Headers;

namespace Project_CallAPI.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly string link = "http://localhost:5091/api/";
        HttpClient _client;

        public SubmissionController()
        {
            _client = new HttpClient();
        }

        [HttpPost]
        public async Task<IActionResult> Add(StudentAssignment dto, IFormFile file)
        {
            var submission = new StudentAssignment()
            {
                AssignmentId = dto.AssignmentId,
                StudentId = dto.StudentId,
                SubmissionDate = DateTime.Now
            };

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(submission.AssignmentId.ToString()), "assignmentId");
            formData.Add(new StringContent(submission.StudentId.ToString()), "studentId");
            formData.Add(new StringContent(submission.SubmissionDate.ToString()), "submissionDate");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                formData.Add(fileContent, "file", file.FileName);
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
            HttpResponseMessage response = await _client.PostAsync(link + "Submission", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Assignment", new { id = submission.AssignmentId });
            }
            return RedirectToAction("Detail", "Assignment", new { id = submission.AssignmentId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(StudentAssignment dto, IFormFile file)
        {
            var submission = new StudentAssignment()
            {
                SubmissionId = dto.SubmissionId,
                AssignmentId = dto.AssignmentId,
                StudentId = dto.StudentId,
                SubmissionDate = DateTime.Now
            };

            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(submission.SubmissionId.ToString()), "submissionId");
            formData.Add(new StringContent(submission.AssignmentId.ToString()), "assignmentId");
            formData.Add(new StringContent(submission.StudentId.ToString()), "studentId");
            formData.Add(new StringContent(submission.SubmissionDate.ToString()), "submissionDate");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                formData.Add(fileContent, "file", file.FileName);
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));

            HttpResponseMessage response = await _client.PutAsync(link + "Submission/" + dto.SubmissionId, formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", "Assignment", new { id = submission.AssignmentId });
            }
            return RedirectToAction("Detail", "Assignment", new { id = submission.AssignmentId });
        }
    }
}
