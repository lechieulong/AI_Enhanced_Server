using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using Model.Live;
using AutoMapper;
using Entity.Live;
using Model.Payment;
using Repository.Live;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_GiftController : ControllerBase
    {
        private readonly IUser_GiftRepository _userGiftRepository;
        private readonly IMapper _Mapper;

        public User_GiftController(IUser_GiftRepository userGiftRepository, IMapper Mapper)
        {
            _userGiftRepository = userGiftRepository;
            _Mapper = Mapper; 
        }

        [HttpGet]
        public async Task<IActionResult> GetUser_Gifts()
        {
            var userGifts = await _userGiftRepository.GetUser_GiftAsync();
            var result=_Mapper.Map<List<User_GiftModel>>(userGifts);
            return Ok(result);
        }

        // GET: api/User_Gift/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser_GiftById(Guid id)
        {
            var userGift = await _userGiftRepository.GetUser_GiftByIDAsync(id);
            var result = _Mapper.Map<User_GiftModel>(userGift);
            if (userGift == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/User_Gift/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser_GiftsByUserId(string userId)
        {
            var userGifts = await _userGiftRepository.GetUser_GiftByUserId(userId);
            var result = _Mapper.Map<List<User_GiftModel>>(userGifts);
            return Ok(result);
        }

        // GET: api/User_Gift/receiver/{receiverId}
        [HttpGet("receiver/{receiverId}")]
        public async Task<IActionResult> GetUser_GiftsByReceiverId(string receiverId)
        {
            var userGifts = await _userGiftRepository.GetUser_GiftByReceiverId(receiverId);
            var result = _Mapper.Map<List<User_GiftModel>>(userGifts);
            return Ok(result);
        }

        // POST: api/User_Gift
        [HttpPost]
        public async Task<IActionResult> AddUser_Gift(User_GiftModel model)
        {
            var userGift = _Mapper.Map<User_Gift>(model);
            userGift.Id = Guid.NewGuid();
            userGift.GiftTime = DateTime.Now;
            var createdUserGift = await _userGiftRepository.AddUser_GiftAsync(userGift);
            model.Id = userGift.Id;
            return Ok(model);
        }

        // PUT: api/User_Gift/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser_Gift(Guid id, User_GiftModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }
            var userGift = _Mapper.Map<User_Gift>(model);

            var updatedUserGift = await _userGiftRepository.UpdateGiftAsync(userGift);
            if (updatedUserGift == null)
            {
                return NotFound();
            }

            return Ok(model);
        }
    }
}
