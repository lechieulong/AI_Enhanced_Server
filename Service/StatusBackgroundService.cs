using Common;
using IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class StatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<StatusBackgroundService> _logger;

        public StatusBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<StatusBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Delay 1 phút
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Checking for schedules with pending statuses...");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var teacherScheduleRepository = scope.ServiceProvider.GetRequiredService<ITeacherScheduleRepository>();

                try
                {
                    // Get all schedules that are pending
                    var pendingSchedules = await teacherScheduleRepository.GetAllPendingAsync();

                    var currentTime = DateTime.Now;

                    // Update statuses of pending schedules if they have expired
                    foreach (var schedule in pendingSchedules)
                    {
                        // Check if the schedule's PendingTime has expired
                        if (schedule.PendingTime.HasValue && currentTime > schedule.PendingTime.Value)
                        {
                            // Update the status back to Available
                            schedule.Status = (int)ScheduleStatus.Available; // Trở lại trạng thái Available
                            schedule.PendingTime = null; // Đặt PendingTime thành null

                            await teacherScheduleRepository.UpdateAsync(schedule); // Lưu các thay đổi vào cơ sở dữ liệu

                            _logger.LogInformation($"Schedule {schedule.Id} pending time has expired and its status has been updated to Available.");
                        }
                    }

                    _logger.LogInformation("Checked all pending schedules.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating schedule statuses.");
                }
            }
        }
    }
}
