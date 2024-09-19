using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Data;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        IMapper _mapper;

        public ClassAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
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
        [Route("{id:int}")]
        public ResponseDto Get(int id)
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
        [Route("{id:int}")]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
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
    }
}
