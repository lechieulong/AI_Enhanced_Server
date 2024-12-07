using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Course
{
    public class AdminResponseDto
    {
        [Required]
        [StringLength(500, ErrorMessage = "Phản hồi không được vượt quá 500 ký tự.")]
        public string AdminResponse { get; set; } // Nội dung phản hồi của admin

        [Required]
        public string Status { get; set; } // Trạng thái báo cáo (Pending, Approved, Rejected)
    }
}
