﻿using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using AutoMapper;
using Model.Live;
using Entity.Live;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamSessionController : ControllerBase
    {
        private readonly IStreamSessionRepository _repository;
        private readonly IMapper _Mapper;

        public StreamSessionController(IStreamSessionRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Streamsession = await _repository.getStreamSessionsIsLive();
            var result = _Mapper.Map<List<StreamSessionModel>>(Streamsession);
            return Ok(result);
        }
        [HttpGet("{liveid}")]
        public async Task<IActionResult> GetStreamSession(Guid liveid)
        {
            var Streamsession = await _repository.getStreamSession(liveid);
            var result = _Mapper.Map<StreamSessionModel>(Streamsession);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateStreamSession(StreamSessionModel model)
        {
            if(model == null)
            {
                return BadRequest(model);
            }
            var streamsessionexist = await _repository.getStreamSession(model.LiveStreamId);
            if (streamsessionexist != null) {

                var result = _Mapper.Map<StreamSessionModel>(streamsessionexist);
                return Ok(result);
            }
            else
            {
            model.Id = Guid.NewGuid();
            model.StartTime = DateTime.Now;
            var Streamsession = _Mapper.Map<StreamSession>(model);
            var result = await _repository.AddStreamSessionAsync(Streamsession);
            return Ok(model);
            }
            
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStreamSession(StreamSessionModel model)
        {
            if (model == null)
            {
                return BadRequest(model);
            }
            var Streamsession = _Mapper.Map<StreamSession>(model);
            var result = await _repository.UpdateStreamSessionAsync(Streamsession);
            return Ok(model);
        }
    }
}
