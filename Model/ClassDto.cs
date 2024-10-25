using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ClassDto
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        //Number of student
        public Guid CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } // Thêm thuộc tính EndDate

        public TimeSpan StartTime { get; set; } // Thời gian bắt đầu lớp học
        public TimeSpan EndTime { get; set; } // Thời gian kết thúc lớp học
        public string ImageUrl { get; set; }


    }
}