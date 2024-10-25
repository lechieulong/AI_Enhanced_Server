using AIIL.Services.Api.Extensions;
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

var builder = WebApplication.CreateBuilder(args);

//Add to support Middleware get
//builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register EmailSenderService and EmailTemplateService
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//DataProtectionTokenProviderOptions chỉ áp dụng cho các loại mã thông báo (token) được tạo ra bởi Identity Token Providers
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

builder.Services.AddScoped<ITestExamRepository, TestExamRepository>();
builder.Services.AddScoped<ITestExamService, TestExamService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseTimelineRepository, CourseTimelineRepository>();
builder.Services.AddScoped<ICourseTimelineDetailRepository, CourseTimelineDetailRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITeacherScheduleRepository, TeacherScheduleRepository>();
builder.Services.AddScoped<IBlogStorageService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("AzureBlobStorage");
    return new BlobStorageService(connectionString);
});

builder.Services.AddScoped<IStreamSessionRepository, StreamSessionRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

// Đăng ký Background Service
builder.Services.AddHostedService<NotificationBackgroundService>();

// Register CORS services
builder.Services.AddCors(options =>
{
    var allowedOrigin = builder.Configuration.GetValue<string>("AllowedOrigins:FrontendUrl");

    options.AddPolicy("AllowMyOrigin",
        policy => policy.WithOrigins(allowedOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AI-Enhanced IELTS Prep API",
        Version = "v1",
        Description = "API for AI-Enhanced IELTS Prep application"
    });

    // Cấu hình Bearer Token cho Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your token"
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
                }
            },
            new string[] {}
        }
    });
});

builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

//Add middleware when neccesary
//app.UseMiddleware<LockoutCheckMiddleware>();

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

        // Gọi SeedSpecializations để thêm dữ liệu nếu cần
        SeedSpecializations(scope.ServiceProvider);
    }
}

void SeedSpecializations(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Kiểm tra nếu bảng Specialization chưa có dữ liệu
        if (!context.Specializations.Any())
        {
            context.Specializations.AddRange(
                new Specialization { Id = Guid.NewGuid(), Name = "Speaking" },
                new Specialization { Id = Guid.NewGuid(), Name = "Writing" },
                new Specialization { Id = Guid.NewGuid(), Name = "Reading" },
                new Specialization { Id = Guid.NewGuid(), Name = "Listening" }
            );

            context.SaveChanges();
        }
    }
}
