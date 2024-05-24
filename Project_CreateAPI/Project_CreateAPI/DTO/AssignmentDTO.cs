namespace Project_CreateAPI.DTO
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Display { get; set; }
        public DateTime? Deadline { get; set; }
        public string? File { get; set; }
    }
}
