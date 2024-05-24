namespace Project_CreateAPI.DTO
{
    public class QuizAttendanceDTO
    {
        public int AttendanceId { get; set; }
        public int QuizId { get; set; }
        public int StudentId { get; set; }
        public int? Score { get; set; }
    }
}
