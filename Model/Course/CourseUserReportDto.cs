using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Course
{
    public class CourseUserReportDto
    {
        public string UserId { get; set; } // ID của người báo cáo
        public Guid CourseId { get; set; } // ID khóa học bị báo cáo
        public string Comment { get; set; } // Nội dung báo cáo
    }
}
