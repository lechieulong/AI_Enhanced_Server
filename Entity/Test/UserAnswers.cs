using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Test
{
    public class UserAnswers
    {
        public Guid Id { get; set; } // Primary Key
        public Guid TestId { get; set; } // Foreign Key to TestExam
        public Guid UserId { get; set; } // Foreign Key to User
        public Guid QuestionId { get; set; } // Foreign Key to Question
        public Guid? AnswerId { get; set; } // Foreign Key to Answer
        public string? AnswerText { get; set; } // The user's provided answer
        public int AttemptNumber { get; set; } 

    }
}
