using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ITeacherRequestRepository
    {
        Task AddRequestAsync(TeacherRequest teacherRequest, UserEducation userEducation);
    }
}
