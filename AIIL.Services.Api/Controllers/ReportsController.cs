using Entity;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        // Tạo mới một báo cáo
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] ReportDTO reportDTO)
        {
            if (reportDTO == null)
            {
                return BadRequest("Invalid report data.");
            }

            var report = new Report
            {
                Id = Guid.NewGuid(),
                UserId = reportDTO.UserId,
                CourseId = reportDTO.CourseId,
                LiveStreamId = reportDTO.LiveStreamId,
                ReportType = reportDTO.ReportType,
                IssueTitle = reportDTO.IssueTitle,
                IssueDescription = reportDTO.IssueDescription,
                AttachmentUrl = reportDTO.AttachmentUrl,
                Priority = reportDTO.Priority,
                FeedbackOption = reportDTO.FeedbackOption,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = "pending" // Mặc định trạng thái là "pending"
            };

            var createdReport = await _reportRepository.CreateReportAsync(report);
            return CreatedAtAction(nameof(GetReportById), new { id = createdReport.Id }, createdReport);
        }

        // Lấy báo cáo theo Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var report = await _reportRepository.GetReportByIdAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report);
        }

        // Lấy tất cả báo cáo
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _reportRepository.GetAllReportsAsync();
            return Ok(reports);
        }

        // Cập nhật báo cáo
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] ReportDTO reportDTO)
        {

            var report = new Report
            {
                Id = id,
                UserId = reportDTO.UserId,
                CourseId = reportDTO.CourseId,
                LiveStreamId = reportDTO.LiveStreamId,
                ReportType = reportDTO.ReportType,
                IssueTitle = reportDTO.IssueTitle,
                IssueDescription = reportDTO.IssueDescription,
                AttachmentUrl = reportDTO.AttachmentUrl,
                Priority = reportDTO.Priority,
                FeedbackOption = reportDTO.FeedbackOption,
                UpdatedAt = DateTime.UtcNow,
                Status = "in_progress" // Cập nhật trạng thái khi sửa
            };

            var updated = await _reportRepository.UpdateReportAsync(report);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Xóa báo cáo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var deleted = await _reportRepository.DeleteReportAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
