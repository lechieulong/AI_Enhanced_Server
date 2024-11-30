using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpPost("set")]
        public IActionResult SetValue([FromQuery] string key, [FromQuery] string value)
        {
            var db = _redis.GetDatabase();
            db.StringSet(key, value);
            return Ok("Value set in Redis");
        }

        [HttpGet("get/{key}")]
        public IActionResult GetValue(string key)
        {
            var db = _redis.GetDatabase();
            var value = db.StringGet(key);
            return value.HasValue ? Ok(value.ToString()) : NotFound("Key not found");
        }
    }

}

