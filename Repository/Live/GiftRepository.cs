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
    }
}