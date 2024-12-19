using System;
using System.Threading;
using System.Threading.Tasks;
using Entity;
using IRepository;
using IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NotificationBackgroundService> _logger;

    public NotificationBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<NotificationBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWork(stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Delay theo yêu cầu
        }
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Checking for users who need notifications...");

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var testExamRepository = scope.ServiceProvider.GetRequiredService<ITestExamRepository>();
            var enrollRepository = scope.ServiceProvider.GetRequiredService<IEnrollmentRepository>();
            var userExamRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var _emailSender= scope.ServiceProvider.GetRequiredService<IEmailSender>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            try
            {
                var tests = await testExamRepository.getAll();

                foreach (var test in tests)
                {
                    DateTime now = DateTime.Now;
                    DateTime startTime = test.StartTime;

                    // Chỉ lấy ngày và giờ
                    DateTime nowDateHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                    DateTime startDateHour = new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.Hour, 0, 0);

                    if (test.TestType == 2 && startDateHour == nowDateHour)
                    {
                        var courses = await enrollRepository.GetEnrollmentsByCourse(test.CourseId);
                        foreach (var course in courses)
                        {
                            var user = await userManager.FindByIdAsync(course.UserId);

                            if (user != null)
                            {
                                await _emailSender.SendEmailAsync(user.Email, "Remind: Final Exam", "It's time for your final exam!");
                            }
                        }
                    }
                }

                _logger.LogInformation("Processed all tests successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing tests.");
            }
        }
    }
}


