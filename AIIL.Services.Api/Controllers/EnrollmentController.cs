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
        private readonly AppDbContext _db; 
        public EnrollmentController(IEnrollmentRepository enrollmentRepository, AppDbContext db)
        {
            _enrollmentRepository = enrollmentRepository;
            _db = db; 
        }

        [HttpPost]
        public async Task<ActionResult<Enrollment>> EnrollUser([FromBody] EnrollmentDto enrollmentRequest)
        {
            if (enrollmentRequest == null)
            {
                return BadRequest("Invalid user or course data.");
            }

            if (enrollmentRequest.CourseId == Guid.Empty || string.IsNullOrEmpty(enrollmentRequest.UserId))
            {
                return BadRequest("CourseId and UserId must be valid.");
            }

            try
            {

                var existingEnrollment = await _db.Enrollments
                    .AnyAsync(e => e.CourseId == enrollmentRequest.CourseId && e.UserId == enrollmentRequest.UserId);

                var newEnrollment = new Enrollment
                {
                    Id = Guid.NewGuid(),
                    CourseId = enrollmentRequest.CourseId,
                    UserId = enrollmentRequest.UserId,
                    ClassId = enrollmentRequest.ClassId,
                    EnrollAt = enrollmentRequest.EnrollAt,
                };

                _db.Enrollments.Add(newEnrollment);

                var course = await _db.Courses.FindAsync(enrollmentRequest.CourseId);
                if (course == null)
                {
                    return NotFound("Khóa học không tồn tại.");
                }
                course.EnrollmentCount++;
                var classEntity = await _db.Classes.FindAsync(enrollmentRequest.ClassId);
                if (classEntity == null)
                {
                    return NotFound("Lớp học không tồn tại.");
                }
                classEntity.EnrollmentCount++;
                await _db.SaveChangesAsync();

                return Ok(newEnrollment);
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
            var enrollment = await _enrollmentRepository.GetEnrollment(courseId, userId);

            return Ok(enrollment != null);
        }
        [HttpGet("classIds")]
        public async Task<ActionResult<List<Guid>>> GetClassIdsByEnrollment(Guid courseId, string userId)
        {
            var classIds = await _enrollmentRepository.GetClassIdsByEnrollment(courseId, userId);

            if (classIds == null || !classIds.Any())
            {
                return NotFound();
            }

            return Ok(classIds);
        }

    }
}
