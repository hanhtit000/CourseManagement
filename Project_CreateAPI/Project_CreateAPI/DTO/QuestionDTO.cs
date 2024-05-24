namespace Project_CreateAPI.DTO
{
    public class QuestionDTO
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string? Question1 { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
    }
}
