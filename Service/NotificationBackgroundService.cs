﻿using System;
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
                                string subject = "Reminder: Your exam is now open!";
                                string examLink = $"https://aiilprep.azurewebsites.net/testDetail/{test.Id}";
                                string body = $@"
            <html>
                <body>
                    <h2>Hello!</h2>
                    <p>This is a friendly reminder that your exam is now open:</p>
                    <p><strong>Exam details:</strong></p>
                    <ul>
                        <li>Exam Name: <strong>{test.TestName}</strong></li>
                        <li>Start Time: <strong>{test.StartTime}</strong></li>
                        <li>Submission Deadline: <strong>{test.EndTime}</strong></li>
                    </ul>
                    <p>Please click the link below to start your exam:</p>
                    <p><a href='{examLink}' target='_blank'>Start the Exam</a></p>
                    <p>Good luck and do your best!</p>
                    <p>Best regards,<br>Your Support Team</p>
                </body>
            </html>";
                                await _emailSender.SendEmailAsync(user.Email, subject, body);
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


