﻿using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Model;
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

        public ClassAPIController(IClassRepository classRepository, IMapper mapper)
        {
            _mapper = mapper;
            _response = new ResponseDto();
            _classRepository = classRepository;
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
                classDto.CourseId == Guid.Empty || // Kiểm tra CourseId có hợp lệ không
                classDto.StartDate == DateTime.MinValue || // Kiểm tra StartDate có hợp lệ không
                classDto.EndDate == DateTime.MinValue || // Kiểm tra EndDate có hợp lệ không
                classDto.EndDate <= classDto.StartDate) // Kiểm tra EndDate phải lớn hơn StartDate
            {
                return BadRequest("Invalid class data.");
            }

            // Chuyển đổi từ ClassDto sang Class entity
            var classEntity = new Class
            {
                Id = Guid.NewGuid(), // Tạo ID mới cho lớp học
                ClassName = classDto.ClassName,
                ClassDescription = classDto.ClassDescription,
                CourseId = classDto.CourseId,
                StartDate = classDto.StartDate,
                EndDate = classDto.EndDate,
                IsEnabled = classDto.IsEnabled // Sử dụng IsEnabled từ ClassDto
            };

            // Gọi repository để tạo lớp học
            var createdClass = await _classRepository.CreateAsync(classEntity);

            // Ánh xạ lại classEntity thành ClassDto để trả về
            var createdClassDto = _mapper.Map<ClassDto>(createdClass);

            return Ok(createdClassDto); // Trả về ClassDto của lớp học đã tạo
        }






        [HttpPut("{classId:guid}")]
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
    }
}
