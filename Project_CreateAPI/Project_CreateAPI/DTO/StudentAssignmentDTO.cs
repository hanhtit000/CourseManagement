namespace Project_CreateAPI.DTO
{
    public class StudentAssignmentDTO
    {
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public int StudentId { get; set; }
        public string? File { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
}
