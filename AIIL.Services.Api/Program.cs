﻿using AIIL.Services.Api.Extensions;
using AIIL.Services.Api.Middlewares;
using AutoMapper;
using Entity;
using Entity.Data;
using IRepository;
using IRepository.Live;
using IService;
using Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Email;
using IRepository;
using Repository;
using Repository.Live;
using Service;
using Repositories;
using OfficeOpenXml;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http.Features;
using AIIL.Services.Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{

    var configuration = ConfigurationOptions.Parse("172.17.0.2"); // Thay đổi theo IP của Docker container
    configuration.AbortOnConnectFail = false;
    configuration.ConnectTimeout = 10000;
    return ConnectionMultiplexer.Connect(configuration);
});


ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,           // Maximum number of retry attempts
            maxRetryDelay: TimeSpan.FromSeconds(10), // Time to wait before retrying
            errorNumbersToAdd: null     // Error numbers that should trigger a retry
        )
    )
);

builder.Services.AddSingleton<RedisService>(sp => new RedisService(builder.Configuration.GetConnectionString("RedisConnection")));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100 MB
});

// Configure Kestrel server to accept larger request bodies
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100_000_000; // 100 MB
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(15);
});

builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserEducationRepository, UserEducationRepository>();
builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
builder.Services.AddScoped<ITeacherRequestRepository, TeacherRequestRepository>();
builder.Services.AddScoped<IBookedScheduleSessionRepository, BookedScheduleSessionRepository>();
builder.Services.AddScoped<ITestExamRepository, TestExamRepository>();
builder.Services.AddScoped<ITestExamService, TestExamService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITeacherScheduleRepository, TeacherScheduleRepository>();
builder.Services.AddScoped<IAzureService, AzureService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();

builder.Services.AddScoped<IBlogStorageService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var azureBlobStorage = config.GetConnectionString("AzureBlobStorage");
    var azureBlobStorageCourse = config.GetConnectionString("AzureBlobStorageCourse");

    // Truyền cả hai Connection String vào Service
    return new BlobStorageService(azureBlobStorage, azureBlobStorageCourse);
});


builder.Services.AddScoped<ILiveStreamRepository, LiveStreamRepository>();
builder.Services.AddScoped<IStreamSessionRepository, StreamSessionRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IUser_GiftRepository, User_GiftRepository>();
builder.Services.AddScoped<IUser_TicketRepository, User_TicketRepository>();
builder.Services.AddScoped<ICourseSkillRepository, CourseSkillRepository>();
builder.Services.AddScoped<ICoursePartRepository, CoursePartRepository>();
builder.Services.AddScoped<ICourseLessonRepository, CourseLessonRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ICourseLessonContentRepository, CourseLessonContentRepository>();
builder.Services.AddScoped<ICourseRatingRepository, CourseRatingRepository>();
builder.Services.AddScoped<IClassFileRepository, ClassFileRepository>();
builder.Services.AddScoped<ITeacherRatingRepository, TeacherRatingRepository>();
builder.Services.AddHostedService<NotificationBackgroundService>();
//builder.Services.AddHostedService<LiveStreamBackgroundService>();
builder.Services.AddHostedService<StatusBackgroundService>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins:FrontendUrls").Get<string[]>();

    options.AddPolicy("AllowMyOrigin",
        policy => policy.WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AI-Enhanced IELTS Prep API",
        Version = "v1",
        Description = "API for AI-Enhanced IELTS Prep application"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new string[] {}
        }
    });
});


builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI-Enhanced IELTS Prep API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowMyOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();
app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }

        DatabaseSeeder.SeedSpecializationsAndUserAsync(scope.ServiceProvider).GetAwaiter().GetResult();
    }
}
var jwtSettings = builder.Configuration.GetSection("ApiSettings:JwtOptions");