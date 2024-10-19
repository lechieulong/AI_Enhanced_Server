using Entity;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly AppDbContext _db;
        public SpecializationRepository(AppDbContext context)
        {
            _db = context;
        }
        public async Task<IEnumerable<Specialization>> GetAllAsync()
        {
            return await _db.Specializations.ToListAsync();
        }
    }
}
