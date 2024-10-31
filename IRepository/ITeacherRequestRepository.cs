using Common;
using Entity;
using Model;
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
        Task<TeacherRequest> GetRequestByUserId(string userId);
        Task<(IEnumerable<TeacherRequestDto> requests, int totalCount)> GetTeacherRequestsAsync(int page, int pageSize, RequestStatusEnum status);
        Task<TeacherRequestDto> GetRequestByRequestId(Guid requestId);
    }
}
