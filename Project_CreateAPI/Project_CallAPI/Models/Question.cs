using System;
using System.Collections.Generic;

namespace Project_CreateAPI.Models
{
    public partial class Question
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string? Question1 { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
    }
}
