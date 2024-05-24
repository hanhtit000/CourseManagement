using System;
using System.Collections.Generic;

namespace Project_CreateAPI.Models
{
    public partial class StudentAssignment
    {
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string? File { get; set; }
        public DateTime? SubmissionDate { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual User Student { get; set; } = null!;
    }
}
