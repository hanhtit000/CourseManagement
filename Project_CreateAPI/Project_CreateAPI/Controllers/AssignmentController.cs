using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Project_CreateAPI.DTO;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ProjectDbContext _context;

        public AssignmentController(IAssignmentRepository assignmentRepository, ProjectDbContext context)
        {
            _assignmentRepository = assignmentRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromForm] AssignmentDTO dto, IFormFile? file)
        {
            var assignment = new Assignment()
            {
                CourseId = dto.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                Display = dto.Display,
                Deadline = dto.Deadline
            };

            if (file != null && file.Length > 0)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Materials");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Materials", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                assignment.File = fileName;
            }

            await _assignmentRepository.CreateAssignment(assignment);
            return Ok("Assignment created successfully!");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromForm] AssignmentDTO dto, IFormFile? file)
        {
            var existingAssignment = await _assignmentRepository.GetAssignmentById(id);
            if(existingAssignment == null)
            {
                return NotFound("Can't find the assignment!");
            }

            existingAssignment.Title = dto.Title;
            existingAssignment.Description = dto.Description;
            existingAssignment.Display = dto.Display;
            existingAssignment.Deadline = dto.Deadline;

            if(file != null && file.Length > 0)
            {
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Materials");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Materials", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingAssignment.File))
                {
                    var previousFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Materials", existingAssignment.File);
                    if (System.IO.File.Exists(previousFilePath))
                    {
                        System.IO.File.Delete(previousFilePath);
                    }
                }

                existingAssignment.File = fileName;         
            }
            await _assignmentRepository.UpdateAssignment(existingAssignment);
            return Ok("Assignment updated successfully!");
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var existingAssignment = await _assignmentRepository.GetAssignmentById(id);
            if (existingAssignment == null)
            {
                return NotFound("Can't find the assignment!");
            }
            return Ok(existingAssignment);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            //Delete child records
            var studentAssignmentsToDelete = _context.StudentAssignments
                .Where(sa => sa.AssignmentId == id);
            _context.StudentAssignments.RemoveRange(studentAssignmentsToDelete);
            _context.SaveChanges();

            //Delete the assignment
            var assignmentToDelete = _context.Assignments.Find(id);
            if (assignmentToDelete == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignmentToDelete);
            _context.SaveChanges();

            return Ok("Assignment and associated records deleted successfully!");
        }
    }
}
