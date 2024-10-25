using System;
using System.Threading;
using System.Threading.Tasks;
using IService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            //var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

            try
            {
                //await emailSenderService.SendEmailRemindMemberAsync("nguyensy23112311@gmail.com", "Check background service.");
                _logger.LogInformation("Reminder email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending reminder email.");
            }
        }
    }
}

