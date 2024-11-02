using Entity.Data;
using Entity.Live;
using IRepository.Live;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Live
{
    public class User_GiftRepository:IUser_GiftRepository
    {
        private readonly AppDbContext _context;

        public User_GiftRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User_Gift>> GetUser_GiftAsync()
        {
            return await _context.User_Gifts.ToListAsync();
        }
        public async Task<User_Gift> GetUser_GiftByIDAsync(Guid id)
        {
            return await _context.User_Gifts.FirstOrDefaultAsync(o=>o.Id.Equals(id));
        }
        public async Task<IEnumerable<User_Gift>> GetUser_GiftByUserId(String id)
        {

            return await _context.User_Gifts
                         .Where(o => o.UserId.Equals(id))
                         .ToListAsync();
        }
        public async Task<IEnumerable<User_Gift>> GetUser_GiftByReceiverId(String id)
        {

            return await _context.User_Gifts
                         .Where(o => o.ReceiverId.Equals(id))
                         .ToListAsync();
        }
        public async Task<User_Gift> AddUser_GiftAsync(User_Gift model)
        {
            var existGift = await GetUser_GiftByIDAsync(model.Id);
            if (existGift == null)
            {
                _context.User_Gifts.Add(model);

                await _context.SaveChangesAsync();
            }
            return model;

        }
        public async Task<User_Gift> UpdateGiftAsync(User_Gift model)
        {

            var existGift = await GetUser_GiftByIDAsync(model.Id);
            if (existGift != null)
            {
                _context.Entry(existGift).CurrentValues.SetValues(model);
            }
            else
            {
                _context.User_Gifts.Add(model);
            }
            await _context.SaveChangesAsync();

            return model;

        }
    }
}
