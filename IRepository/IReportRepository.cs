using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IReportRepository
    {
        Task<Report> CreateReportAsync(Report report);
        Task<Report> GetReportByIdAsync(Guid reportId);
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<bool> UpdateReportAsync(Report report);
        Task<bool> DeleteReportAsync(Guid reportId);
    }
}
