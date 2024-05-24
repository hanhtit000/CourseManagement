using System;
using System.Collections.Generic;

namespace Project_CreateAPI.Models
{
    public partial class QuizAttendance
    {
        public int AttendanceId { get; set; }
        public int QuizId { get; set; }
        public int StudentId { get; set; }
        public int? Score { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
        public virtual User Student { get; set; } = null!;
    }
}
