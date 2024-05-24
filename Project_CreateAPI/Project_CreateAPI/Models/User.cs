using System;
using System.Collections.Generic;

namespace Project_CreateAPI.Models
{
    public partial class User
    {
        public User()
        {
            CourseEnrollments = new HashSet<CourseEnrollment>();
            Courses = new HashSet<Course>();
            QuizAttendances = new HashSet<QuizAttendance>();
            StudentAssignments = new HashSet<StudentAssignment>();
        }

        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<QuizAttendance> QuizAttendances { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
