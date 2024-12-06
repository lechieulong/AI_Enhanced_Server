namespace Common
{
    public enum SkillTypeEnum
    {
        Reading,    // 0
        Listening,   // 1
        Writing,   // 2
        Speaking, // 3
    }

    public enum QuestionTypeENum
    {
        Matching,    // 0
        Filling,   // 1
        Mutiple,   // 2
        Radio, // 3
        TrueFalse, // 3
    }

    public enum TypeCorrect
    {
        False,    // 0
        True,   // 1
        NotGiven,   // 2
    }

    public enum ScheduleStatus
    {
        Available = 0,
        Pending = 1,
        Booked = 2
    }

    public enum RequestStatusEnum
    {
        Pending = 0,
        Approve = 1,
        Reject = 2
    }
}
