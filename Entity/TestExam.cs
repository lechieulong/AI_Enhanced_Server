
namespace Entity
{
    public class TestExam
    {
        public Guid Id { get; set; }
        public string TestName { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public ICollection<PartSkill> PartSkills { get; set; }
        public ICollection<QuestionTypePart> QuestionTypeParts { get; set; }
    }

}
