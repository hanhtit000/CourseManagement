using System;
using System.Collections.Generic;

namespace Project_CreateAPI.Models
{
    public partial class Course
    {
        public Course()
        {
            Assignments = new HashSet<Assignment>();
            CourseEnrollments = new HashSet<CourseEnrollment>();
            Quizzes = new HashSet<Quiz>();
        }

        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int LecturerId { get; set; }

        public virtual User Lecturer { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public virtual ICollection<Quiz> Quizzes { get; set; }
    }
}
