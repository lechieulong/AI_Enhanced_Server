using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity;
using Entity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Repositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly AppDbContext _db; // DbContext để truy cập cơ sở dữ liệu

        // Thêm AppDbContext vào constructor
        public EnrollmentController(IEnrollmentRepository enrollmentRepository, AppDbContext db)
        {
            _enrollmentRepository = enrollmentRepository;
            _db = db; // Khởi tạo DbContext
        }

        // POST: api/Enrollment
        [HttpPost]
        public async Task<ActionResult<Enrollment>> EnrollUser([FromBody] EnrollmentDto enrollmentRequest)
        {
            if (enrollmentRequest == null)
            {
                return BadRequest("Invalid user or course data.");
            }

            // Kiểm tra CourseId và UserId có giá trị hợp lệ không
            if (enrollmentRequest.CourseId == Guid.Empty || string.IsNullOrEmpty(enrollmentRequest.UserId))
            {
                return BadRequest("CourseId and UserId must be valid.");
            }

            try
            {
                var existingEnrollment = await _db.Enrollments
                    .AnyAsync(e => e.CourseId == enrollmentRequest.CourseId && e.UserId == enrollmentRequest.UserId);

                if (existingEnrollment)
                {
                    return BadRequest("Người dùng đã ghi danh vào khóa học này.");
                }

                // Tạo một đối tượng Enrollment mới với Id tự động
                var newEnrollment = new Enrollment
                {
                    Id = Guid.NewGuid(), // Tạo GUID mới
                    CourseId = enrollmentRequest.CourseId,
                    UserId = enrollmentRequest.UserId,
                    ClassId = enrollmentRequest.ClassId,
                    // Nếu có các trường khác mà bạn muốn khởi tạo, hãy thêm vào đây
                };

                // Ghi danh người dùng vào khóa học
                _db.Enrollments.Add(newEnrollment);
                await _db.SaveChangesAsync(); // Lưu vào cơ sở dữ liệu

                return Ok(newEnrollment); // Trả về đối tượng Enrollment đã được ghi danh
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Enrollment/Course/{courseId}
        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByCourse(Guid courseId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourse(courseId);
            if (enrollments == null)
            {
                return NotFound();
            }

            return Ok(enrollments);
        }

        // GET: api/Enrollment/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollmentsByUser(string userId)
        {
            var enrollments = await _enrollmentRepository.GetEnrollmentsByUser(userId);
            if (enrollments == null)
            {
                return NotFound();
            }

            return Ok(enrollments);
        }

        // GET: api/Enrollment/check
        [HttpGet("check")]
        public async Task<IActionResult> CheckEnrollment(Guid courseId, string userId)
        {
            // Kiểm tra xem user đã đăng ký khóa học với courseId và userId chưa
            var enrollment = await _enrollmentRepository.GetEnrollment(courseId, userId);

            // Trả về kết quả check (true/false) và classId nếu có
            return Ok(new
            {
                isEnrolled = enrollment != null,
                classId = enrollment?.ClassId // Trả về classId nếu enrollment tồn tại, nếu không trả về null
            });
        }

    }
}
