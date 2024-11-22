using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Microsoft.AspNetCore.Authorization;
using Model.Utility;
using Entity.CourseFolder;
using Microsoft.EntityFrameworkCore;
using Repository;
using AutoMapper;
using Model.Course;
using System.Security.Claims;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _repository;
        private readonly IClassRepository _classRepository;
        private readonly ICourseSkillRepository _courseSkillRepository;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;

        public CoursesController(ICourseRepository repository, IClassRepository classRepository, ICourseSkillRepository courseSkillRepository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _classRepository = classRepository;
            _courseSkillRepository = courseSkillRepository;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _repository.GetAllAsync();

            //if (courses == null || !courses.Any())
            //{
            //    return NotFound();
            //}

            var coursesDto = _mapper.Map<List<GetCourseListDto>>(courses);

            return Ok(coursesDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseDto courseDto)
        {
            if (courseDto == null || string.IsNullOrWhiteSpace(courseDto.CourseName) ||
                string.IsNullOrWhiteSpace(courseDto.Content) || courseDto.Hours <= 0 ||
                courseDto.Days <= 0 || courseDto.Categories == null ||
                !courseDto.Categories.Any() || courseDto.Price <= 0 ||
                string.IsNullOrWhiteSpace(courseDto.UserId))
            {
                return BadRequest("Invalid course data.");
            }

            var course = new Course
            {
                Id = Guid.NewGuid(),
                CourseName = courseDto.CourseName,
                Content = courseDto.Content,
                Hours = courseDto.Hours,
                Days = courseDto.Days,
                Categories = courseDto.Categories,
                Price = courseDto.Price,
                UserId = courseDto.UserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsEnabled = false
            };

            await _repository.CreateAsync(course);

            foreach (var category in courseDto.Categories)
            {
                var courseSkill = new CourseSkill
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    Type = category,
                    Description = GetSkillDescription(category)
                };

                await _courseSkillRepository.AddAsync(courseSkill);
            }

            return Ok(course);
        }

        private string GetSkillDescription(string category)
        {
            return category switch
            {
                "0" => "Reading",
                "1" => "Listening",
                "2" => "Writing",
                "3" => "Speaking",
                _ => "Unknown skill"
            };
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Course course)
        {
            if (id != course.Id)
            {
                return BadRequest("Course ID mismatch.");
            }

            await _repository.UpdateAsync(course);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("course-info/{courseId}")]
        public async Task<IActionResult> GetCourseInfo(Guid courseId)
        {
            var course = await _repository.GetByIdAsync(courseId);
            var courseDto = _mapper.Map<GetCourseListDto>(course);
            if (course == null)
            {
                _response.Result = null;
                _response.Message = "No course found.";
                return Ok(_response);
            }
            _response.Result = courseDto;
            return Ok(_response);
        }

        [HttpGet("{courseId:guid}/classes")]
        public async Task<IActionResult> GetClassesByCourseId(Guid courseId)
        {
            var classes = await _classRepository.GetByCourseIdAsync(courseId);
            return classes == null || !classes.Any() ? NotFound("No classes found.") : Ok(classes);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllCourseByUserId(string userId)
        {
            var courses = await _repository.GetAllCourseByUserIdAsync(userId);
            return courses == null || !courses.Any() ? NotFound("No courses found.") : Ok(courses);
        }

        [HttpGet("created-courses")]
        [Authorize]
        public async Task<IActionResult> GetCreatedCourses()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authorized.");
            }

            var courses = await _repository.GetCreatedCourses(userId);

            if (courses == null || !courses.Any())
            {
                return Ok("No courses found.");
            }

            var coursesDto = _mapper.Map<List<GetCourseListDto>>(courses);

            return Ok(coursesDto);
        }

        [HttpGet("check-lecturer/{courseId}")]
        public async Task<IActionResult> CheckLecturerOfCourse(Guid courseId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _response.Result = false;
                _response.Message = "User is does not login.";
                return Ok(_response);
            }
            bool isLecturer = await _repository.CheckLecturerOfCourse(userId, courseId);

            if (isLecturer)
            {
                _response.Result = true;
                _response.Message = "User is the lecturer of this course.";
                return Ok(_response);
            }
            else
            {
                _response.Result = false;
                _response.Message = "User is not the lecturer of this course.";
                return Ok(_response);
            }
        }


        [HttpGet("enabled")]
        public async Task<IActionResult> GetEnabledCourses()
        {
            var courses = await _repository.GetAllEnabledCoursesAsync();
            return courses == null || !courses.Any() ? NotFound("No enabled courses found.") : Ok(courses);
        }

        [HttpGet("disabled")]
        public async Task<IActionResult> GetDisabledCourses()
        {
            var courses = await _repository.GetAllDisabledCoursesAsync();
            return courses == null || !courses.Any() ? NotFound("No disabled courses found.") : Ok(courses);
        }

        [HttpGet("{id:guid}/enabled")]
        public async Task<IActionResult> IsCourseEnabled(Guid id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            return Ok(new { course.Id, course.IsEnabled });
        }

        [HttpPut("{courseId:guid}/enabled")]
        public async Task<IActionResult> UpdateCourseEnabledStatus(Guid courseId, [FromBody] bool isEnabled)
        {
            var course = await _repository.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            await _repository.UpdateCourseEnabledStatusAsync(courseId, isEnabled);
            return Ok(new { courseId, IsEnabled = isEnabled });
        }

        [HttpGet("courseLessonContent/{courseLessonContentId}/courseId")]
        public async Task<IActionResult> GetCourseIdByLessonContentId(Guid courseLessonContentId)
        {
            var courseId = await _repository.GetCourseIdByLessonContentIdAsync(courseLessonContentId);

            if (courseId == null)
                return NotFound("Course ID not found for the provided CourseLessonContent ID.");

            return Ok(new { CourseId = courseId });
        }
        // CoursesController.cs
        [HttpGet("DescriptionBySkill/{skillId:guid}")]
        public async Task<IActionResult> GetDescriptionBySkillId(Guid skillId)
        {
            var courseSkill = await _courseSkillRepository.GetBySkillIdAsync(skillId);

            if (courseSkill == null)
            {
                return NotFound("No description found for the provided Skill ID.");
            }

            return Ok(new { Description = courseSkill.Description });
        }

    }
}
