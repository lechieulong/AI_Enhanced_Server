namespace Model
{
    public class CourseTimelineDetailDto
    {
        public Guid Id { get; set; }
        public Guid CourseTimelineId { get; set; }
        public string Title { get; set; }
        public string VideoUrl { get; set; }
        public string Topic { get; set; }
        public bool IsEnabled { get; set; } = true; // Mặc định là true (enabled)

    }
}
