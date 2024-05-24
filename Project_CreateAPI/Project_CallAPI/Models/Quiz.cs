using System;
using System.Collections.Generic;

namespace Project_CreateAPI.Models
{
    public partial class Quiz
    {
        public Quiz()
        {
            Questions = new HashSet<Question>();
            QuizAttendances = new HashSet<QuizAttendance>();
        }

        public int QuizId { get; set; }
        public int CourseId { get; set; }
        public string? Title { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuizAttendance> QuizAttendances { get; set; }
    }
}
