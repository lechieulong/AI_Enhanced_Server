using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IClassRepository
    {
        Task<ClassDto> CreateAsync(ClassDto newClass);
        Task<ClassDto> GetByIdAsync(int id);
        Task<IEnumerable<ClassDto>> GetAllAsync();
        Task<ClassDto> UpdateAsync(ClassDto updatedClass);
        Task<bool> DeleteAsync(int id);
    }
}
