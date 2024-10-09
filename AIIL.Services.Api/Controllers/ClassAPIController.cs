using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Entity.Data;
using Repository;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IClassRepository _classRepository;
        private readonly ResponseDto _response;
        IMapper _mapper;

        public ClassAPIController(IClassRepository classRepository, AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _classRepository = classRepository;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Class> objList = _db.Classes.ToList();
                _response.Result = _mapper.Map<IEnumerable<ClassDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public ResponseDto Get(Guid id)
        {
            try
            {
                Class obj = _db.Classes.First(u => u.Id == id);
                _response.Result = _mapper.Map<ClassDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ClassDto ClassDto)
        {
            try
            {
                Class obj = _mapper.Map<Class>(ClassDto);
                _db.Classes.Add(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ClassDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ClassDto ClassDto)
        {
            try
            {
                Class obj = _mapper.Map<Class>(ClassDto);
                _db.Classes.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ClassDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(Guid id)
        {
            try
            {
                Class obj = _db.Classes.First(c => c.Id == id);
                _db.Classes.Remove(obj);
                _db.SaveChanges();

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
                response.Result = classList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}
