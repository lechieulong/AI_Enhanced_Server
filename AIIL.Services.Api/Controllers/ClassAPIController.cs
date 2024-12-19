using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Model;
using NPOI.Util;
using Repositories;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassAPIController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IClassRepository _classRepository;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly ITestExamRepository _testExamRepository;
        public ClassAPIController(IClassRepository classRepository, IMapper mapper, ITestExamRepository testExamRepository, IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _response = new ResponseDto();
            _classRepository = classRepository;
            _testExamRepository = testExamRepository; 
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                var classList = await _classRepository.GetAllAsync();
                _response.Result = _mapper.Map<IEnumerable<ClassDto>>(classList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("{id:Guid}")]
        public async Task<ResponseDto> Get(Guid id)
        {
            try
            {
                var classEntity = await _classRepository.GetByIdAsync(id);
                if (classEntity == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Class not found.";
                    return _response;
                }

                _response.Result = _mapper.Map<ClassDto>(classEntity);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] CreateClassDto classDto)
            {

            if (classDto == null ||
                string.IsNullOrWhiteSpace(classDto.ClassName) ||
                string.IsNullOrWhiteSpace(classDto.ClassDescription) ||
                classDto.CourseId == Guid.Empty ||
                string.IsNullOrWhiteSpace(classDto.StartDate) ||
                string.IsNullOrWhiteSpace(classDto.EndDate))
            {
                return BadRequest("Invalid class data.");
            }

            var classEntity = new Class
                {
                    Id = Guid.NewGuid(),
                    ClassName = classDto.ClassName,
                    ClassDescription = classDto.ClassDescription,
                    CourseId = classDto.CourseId,
                    StartDate = classDto.StartDate,
                    EndDate = classDto.EndDate,
                    IsEnabled = classDto.IsEnabled,
                    ImageUrl = classDto.ImageUrl,
            };

                var createdClass = await _classRepository.CreateAsync(classEntity);
                var createdClassDto = _mapper.Map<CreateClassDto>(createdClass);

                return Ok(createdClassDto);
            }

        [HttpPut("update/{classId:guid}")]
        public async Task<ResponseDto> Put(Guid classId, [FromBody] ClassDto classDto)
        {
            try
            {
                var updatedClass = await _classRepository.UpdateAsync(classId, classDto);
                if (updatedClass == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Class not found.";
                    return _response;
                }

                _response.Result = _mapper.Map<ClassDto>(updatedClass);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpDelete("{id:Guid}")]
        public async Task<ResponseDto> Delete(Guid id)
        {
            try
            {
                // Kiểm tra xem lớp có tồn tại trong cơ sở dữ liệu không
                var classEntity = await _classRepository.GetByIdAsync(id);
                if (classEntity == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Class not found.";
                    return _response;
                }

                // Lấy danh sách Enrollment có classId trùng với id cần xoá
                var enrollmentsToDelete = await _enrollmentRepository.GetByClassIdAsync(id);

                // Xóa thông tin Enrollment có liên quan
                if (enrollmentsToDelete.Any())
                {
                    await _enrollmentRepository.DeleteRangeAsync(enrollmentsToDelete);
                }

                // Xóa lớp
                await _classRepository.DeleteAsync(id);

                _response.IsSuccess = true; // Trả về trạng thái thành công
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpGet("course/{courseId:Guid}/classes")]
        public async Task<ActionResult<ResponseDto>> GetByCourseId(Guid courseId)
        {
            var response = new ResponseDto();
            try
            {
                var classList = await _classRepository.GetByCourseIdAsync(courseId);
                if (classList == null || !classList.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No classes found for the specified course ID.";
                    return NotFound(response);
                }
                response.Result = _mapper.Map<IEnumerable<ClassDto>>(classList);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("classes/{teacherId:guid}")]
        public async Task<ActionResult<ResponseDto>> GetClassesByTeacherId(Guid teacherId)
        {
            var response = new ResponseDto();
            try
            {
                var classList = await _classRepository.GetByTeacherIdAsync(teacherId.ToString());
                if (classList == null || !classList.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No classes found for the specified teacher ID.";
                    return NotFound(response);
                }
                response.IsSuccess = true;
                response.Result = _mapper.Map<IEnumerable<ClassDto>>(classList);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{classId:guid}/enabled")]
        public async Task<IActionResult> UpdateClassEnabledStatus(Guid classId, [FromBody] bool isEnabled)
        {
            var classEntity = await _classRepository.GetByIdAsync(classId);
            if (classEntity == null)
            {
                return NotFound("Class not found.");
            }

            await _classRepository.UpdateClassEnabledStatusAsync(classId, isEnabled);
            return Ok(new { classId, IsEnabled = isEnabled });
        }

        [HttpGet("{classId:guid}/enabled")]
        public async Task<IActionResult> CheckClassEnabledStatus(Guid classId)
        {
            try
            {
                var classEntity = await _classRepository.GetByIdAsync(classId);
                if (classEntity == null)
                {
                    return NotFound("Class not found.");
                }

                return Ok(new { classId, IsEnabled = classEntity.IsEnabled });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto { IsSuccess = false, Message = ex.Message });
            }
        }
        [HttpGet("GetTestExamsByClassId/{classId}")]
        public async Task<IActionResult> GetTestExamsByClassId(Guid classId)
        {
            // Gọi repository để lấy danh sách các bài kiểm tra từ database
            var testExams = await _testExamRepository.GetTestExamsByClassIdAsync(classId);

            if (testExams == null || testExams.Count == 0)
            {
                return NotFound("No TestExams found for the given ClassId.");
            }

            return Ok(testExams);
        }

        [HttpGet("unenrolled")]
        public async Task<IActionResult> GetUnenrolledClasses([FromQuery] Guid courseId, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                var unenrolledClasses = await _classRepository.GetUnenrolledClassesAsync(courseId, userId);
                return Ok(unenrolledClasses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
