using JobMasterApi.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResumeUploadApi.Data;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services;
using ResumeUploadApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ✅ SQL Server connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// ✅ Identity
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// ✅ JWT Authentication

builder.AddAuth();

// ✅ Service Registration
builder.Services.AddScoped<IGptService, GptService>();
builder.Services.AddScoped<IResumeService, ResumeService>();

builder.Services.AddAuthorization();

// ✅ CORS (optional)
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
    );
});

// ✅ Controllers
builder.Services.AddControllers();

// ✅ Swagger + JWT Auth + Examples
builder.Services.AddEndpointsApiExplorer();

builder.AddSwaggerConfigs();
var app = builder.Build();

// ✅ Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JobMaster API V1");
    c.RoutePrefix = string.Empty; // Makes Swagger available at root URL
});

// ✅ Middleware pipeline
app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthentication(); // 🔐 Important: UseAuthentication before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
