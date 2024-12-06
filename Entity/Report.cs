using Entity.CourseFolder;
using Entity.Live;
using System;

namespace Entity
{
    public class Report
    {
        // Khóa chính của báo cáo
        public Guid Id { get; set; }

        // ID người dùng gửi báo cáo
        public string UserId { get; set; }

        // ID khóa học nếu báo cáo liên quan đến khóa học
        public Guid? CourseId { get; set; }

        // ID LiveStream nếu báo cáo liên quan đến lớp học trực tuyến
        public Guid? LiveStreamId { get; set; }

        // Loại báo cáo: website, course, teacher
        public string ReportType { get; set; }

        // Tiêu đề vấn đề
        public string IssueTitle { get; set; }

        // Mô tả chi tiết vấn đề
        public string IssueDescription { get; set; }

        // Đường dẫn tệp đính kèm nếu có
        public string AttachmentUrl { get; set; }

        // Độ ưu tiên của báo cáo: high, medium, low
        public string Priority { get; set; }

        // Tùy chọn cho phản hồi từ hệ thống
        public bool FeedbackOption { get; set; }

        // Trạng thái của báo cáo: pending, in_progress, resolved, closed
        public string Status { get; set; }

        // Thời gian báo cáo được tạo
        public DateTime CreatedAt { get; set; }

        // Thời gian báo cáo được cập nhật lần cuối
        public DateTime UpdatedAt { get; set; }

        // Thời gian báo cáo được giải quyết
        public DateTime? ResolvedAt { get; set; }

    }
}
