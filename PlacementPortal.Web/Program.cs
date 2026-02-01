using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.BLL;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.Hubs;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Repository;
using PlacementPortal.BLL.Service;
using PlacementPortal.BLL.Strategies;
using PlacementPortal.DAL.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Authentication/Login"; // Adjust this path as per your controller/action
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddDataProtection();

builder.Services.AddSignalR();

builder.Services.AddDbContext<PlacementPortalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfire(config => config
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfireServer();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IGlobalService, GlobalService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<ProtectorHelper>();
builder.Services.AddScoped<IDashboardStrategy, TPODashboardStrategy>();
builder.Services.AddScoped<IDashboardStrategy, StudentDashboardStrategy>();
builder.Services.AddScoped<IDashboardStrategy, AdminDashboardStrategy>();
builder.Services.AddScoped<DashboardStrategyFactory>();
builder.Services.AddScoped<IJobDeadlineReminderService, JobDeadlineReminderService>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<NotificationHub>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<IMFAAuthenticator, MFAAuthenticator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseHangfireDashboard("/hangfire");

app.MapHub<NotificationHub>("/notificationHub");

RecurringJob.AddOrUpdate<IJobDeadlineReminderService>(
    "job-deadline-reminder",
    job => job.SendDeadlineReminderEmailsAsync(),
    "30 23   * * *"
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");
app.Run();
