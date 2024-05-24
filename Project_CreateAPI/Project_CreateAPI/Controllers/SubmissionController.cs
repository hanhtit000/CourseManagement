using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Project_CreateAPI.DTO;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubmissionController : ControllerBase
    {
        private readonly IStudentAssignmentRepository _studentAssignmentRepository;

        public SubmissionController(IStudentAssignmentRepository studentAssignmentRepository)
        {
            _studentAssignmentRepository = studentAssignmentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddSubmission([FromForm] StudentAssignmentDTO dto, IFormFile file)
        {
            var submission = new StudentAssignment()
            {
                SubmissionId = dto.SubmissionId,
                AssignmentId = dto.AssignmentId,
                StudentId = dto.StudentId,
                SubmissionDate = dto.SubmissionDate
            };

            if (file != null && file.Length > 0)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                submission.File = fileName;
            }

            await _studentAssignmentRepository.AddSubmission(submission);
            return Ok("Submission created successfully!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubmission(int id, [FromForm] StudentAssignmentDTO dto, IFormFile? file)
        {
            var existingSubmission = await _studentAssignmentRepository.GetSubmissionById(id);
            if (existingSubmission == null)
            {
                return NotFound("Can't find the submission!");
            }

            existingSubmission.SubmissionDate = dto.SubmissionDate;

            if (file != null && file.Length > 0)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingSubmission.File))
                {
                    var previousFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Submissions", existingSubmission.File);
                    if (System.IO.File.Exists(previousFilePath))
                    {
                        System.IO.File.Delete(previousFilePath);
                    }
                }

                existingSubmission.File = fileName;
            }
            await _studentAssignmentRepository.UpdateSubmission(existingSubmission);
            return Ok("Submission updated successfully!");
        }
    }
}
