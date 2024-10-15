using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IUserEducationRepository
    {
        Task<IEnumerable<UserEducation>> GetAllAsync();
        Task<UserEducation> GetByIdAsync(string teacherId);
        Task<UserEducation> CreateAsync(UserEducation userEducation);
        Task<UserEducation> UpdateAsync(UserEducation userEducation);
        Task DeleteAsync(string teacherId);
    }
}
