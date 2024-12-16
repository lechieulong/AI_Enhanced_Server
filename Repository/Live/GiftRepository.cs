using Entity.Data;
using Entity.Live;
using IRepository.Live;
using Microsoft.EntityFrameworkCore;
using Model.Utility;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Live;

namespace Repository.Live
{
    public class GiftRepository : IGiftRepository
    {
        private readonly AppDbContext _context;

        public GiftRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Gift>> GetGiftAsync()
        {
            return await _context.Gifts.ToListAsync();
        }
        public async Task<Gift> GetGiftByIDAsync(Guid id)
        {
            return await _context.Gifts.FirstOrDefaultAsync(o => o.Id.Equals(id));
        }
        public async Task<Gift> AddGiftAsync(Gift model)
        {
            var existGift = await GetGiftByIDAsync(model.Id);
            if (existGift == null)
            {
                _context.Gifts.Add(model);

                await _context.SaveChangesAsync();
            }
            return model;

        }
        public async Task<Gift> UpdateGiftAsync(Gift model)
        {

            var existGift = await GetGiftByIDAsync(model.Id);
            if (existGift != null)
            {
                _context.Entry(existGift).CurrentValues.SetValues(model);
            }
            else
            {
                _context.Gifts.Add(model);
            }
            await _context.SaveChangesAsync();

            return model;

        }
        public async Task<Gift> DeleteAsync(Guid id)
        {

            var existGift = await GetGiftByIDAsync(id);
            if (existGift != null)
            {
                _context.Gifts.Remove(existGift);
            }
            else
            {
                return null;
            }
            await _context.SaveChangesAsync();

            return existGift;

        }
        public async Task<(IEnumerable<Gift> users, int totalCount)> GetUsersAsync(int page, int pageSize, string? searchQuery)
        {

            var totalCount = await _context.Gifts.CountAsync();
            var gifts= new List<Gift>();
            if (searchQuery != null)
            {
                 gifts = await _context.Gifts.OrderBy(u => u.Price).Where(u => u.Name.Contains(searchQuery)).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            }
            else
            {
             gifts = await _context.Gifts.OrderBy(u => u.Price).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            }

                

            return (gifts, totalCount);
        }
    }
}