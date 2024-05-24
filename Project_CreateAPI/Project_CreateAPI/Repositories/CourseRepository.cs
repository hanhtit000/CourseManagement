using Microsoft.EntityFrameworkCore;
using Project_CreateAPI.Models;

namespace Project_CreateAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ProjectDbContext _context;

        public CourseRepository(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return _context.Courses.AsQueryable();
        }

        public async Task AddCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Course> GetCourseById(int id)
        {
            return await _context.Courses.Include(c => c.Lecturer).Include(c => c.Assignments).Include(c => c.CourseEnrollments).Include(c => c.Quizzes).ThenInclude(q => q.Questions).Include(c => c.Quizzes).ThenInclude(q => q.QuizAttendances).FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public  async Task UpdateCourse(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task EnrollCourse(int courseId, int studentId)
        {
            CourseEnrollment enrollment = new CourseEnrollment()
            {
                CourseId = courseId,
                StudentId = studentId
            };
            _context.CourseEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UnenrollCourse(int courseId, int studentId)
        {
            var enrollment = await _context.CourseEnrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);
            if (enrollment != null)
            {
                _context.CourseEnrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CourseEnrollment>> GetAllEnrollments()
        {
            return _context.CourseEnrollments.AsQueryable();
        }
    }
}
