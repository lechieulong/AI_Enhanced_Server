using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Model;
using NPOI.Util;
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
        private readonly IClassRepository _classRepository;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly ITestExamRepository _testExamRepository;
        public ClassAPIController(IClassRepository classRepository, IMapper mapper, ITestExamRepository testExamRepository)
        {
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
        public async Task<IActionResult> Create([FromBody] ClassDto classDto)
        {
            if (classDto == null ||
                string.IsNullOrWhiteSpace(classDto.ClassName) ||
                string.IsNullOrWhiteSpace(classDto.ClassDescription) ||
                classDto.CourseId == Guid.Empty ||
                classDto.StartDate == DateTime.MinValue ||
                classDto.EndDate == DateTime.MinValue ||
                classDto.EndDate <= classDto.StartDate)
            {
                return BadRequest("Invalid class data.");
            }

            var formattedStartDate = classDto.StartDate.ToString("dd/MM/yyyy");
            var formattedEndDate = classDto.EndDate.ToString("dd/MM/yyyy");

            var classEntity = new Class
            {
                Id = Guid.NewGuid(),
                ClassName = classDto.ClassName,
                ClassDescription = classDto.ClassDescription,
                CourseId = classDto.CourseId,
                StartDate = DateTime.ParseExact(formattedStartDate, "dd/MM/yyyy", null),
                EndDate = DateTime.ParseExact(formattedEndDate, "dd/MM/yyyy", null),
                IsEnabled = classDto.IsEnabled
            };

            var createdClass = await _classRepository.CreateAsync(classEntity);
            var createdClassDto = _mapper.Map<ClassDto>(createdClass);

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
                var classEntity = await _classRepository.GetByIdAsync(id);
                if (classEntity == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Class not found.";
                    return _response;
                }

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
    }
}
