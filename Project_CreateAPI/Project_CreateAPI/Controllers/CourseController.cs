using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project_CreateAPI.DTO;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ProjectDbContext _context;

        public CourseController(ICourseRepository courseRepository, ProjectDbContext context)
        {
            _courseRepository = courseRepository;
            _context = context;
        }

        //Get all courses
        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseRepository.GetAllCourses();
            if(courses != null)
            {              
                return Ok(courses);
            }
            return BadRequest("Can't find list courses");
        }

        //Add a course
        [HttpPost]
        public async Task<IActionResult> AddCourse(CourseDTO dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                LecturerId = dto.LecturerId
            };
            await _courseRepository.AddCourse(course);
            return Ok("Course added successfully!");
        }

        //Update a course
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, CourseDTO updatedCourse)
        {
            var course = await _courseRepository.GetCourseById(id);
            if(course == null)
            {
                return NotFound("Can't find the course!");
            }
            course.Title = updatedCourse.Title;
            course.Description = updatedCourse.Description;
            await _courseRepository.UpdateCourse(course);
            return Ok("Course updated successfully!");
        }

        //Get the specific course by id
        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<ActionResult<Course>> GetCourseById(int id)
        {
            var course = await _courseRepository.GetCourseById(id);
            if (course == null)
            {
                return NotFound("Can't find the course!");
            }
            return Ok(course);
        }

        //Enroll a course with its id
        [HttpPost("{courseId}/enroll")]
        public async Task<IActionResult> EnrollCourse(int courseId, int studentId)
        {
            try
            {
                await _courseRepository.EnrollCourse(courseId, studentId);
                return Ok("Course enrolled successfully!");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Enrollment failed: {ex.Message}");
            }
        }

        //Unenroll a course with its id
        [HttpPost("{courseId}/unenroll")]
        public async Task<IActionResult> UnenrollCourse(int courseId, int studentId)
        {
            try
            {
                await _courseRepository.UnenrollCourse(courseId, studentId);
                return Ok("Course unenrolled successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unenrollment failed: {ex.Message}");
            }
        }

        //Get all enrollments
        [HttpGet("enrollments")]
        [EnableQuery]
        public async Task<IActionResult> GetAllEnrollments()
        {
            var enrollments = await _courseRepository.GetAllEnrollments();
            if(enrollments != null)
            {
                return Ok(enrollments);
            }
            return NotFound("Can't get list enrollments!");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            //Delete child records
            var assignmentsToDelete = _context.Assignments.Where(a => a.CourseId == id);
            var studentAssignmentsToDelete = _context.StudentAssignments
                .Where(sa => assignmentsToDelete.Any(a => a.AssignmentId == sa.AssignmentId));
            _context.StudentAssignments.RemoveRange(studentAssignmentsToDelete);
            _context.Assignments.RemoveRange(assignmentsToDelete);

            var quizzesToDelete = _context.Quizzes.Where(q => q.CourseId == id);
            var quizIdsToDelete = quizzesToDelete.Select(q => q.QuizId).ToList();

            var quizAttendancesToDelete = _context.QuizAttendances.Where(qa => quizIdsToDelete.Contains(qa.QuizId));
            _context.QuizAttendances.RemoveRange(quizAttendancesToDelete);

            var questionsToDelete = _context.Questions.Where(q => quizIdsToDelete.Contains(q.QuizId));
            _context.Questions.RemoveRange(questionsToDelete);

            _context.Quizzes.RemoveRange(quizzesToDelete);

            var enrollmentsToDelete = _context.CourseEnrollments.Where(ce => ce.CourseId == id);
            _context.CourseEnrollments.RemoveRange(enrollmentsToDelete);

            _context.SaveChanges();

            //Delete the course record
            var courseToDelete = _context.Courses.Find(id);
            if (courseToDelete == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(courseToDelete);
            _context.SaveChanges();

            return Ok("Course and associated records deleted successfully!");
        }
    }
}
