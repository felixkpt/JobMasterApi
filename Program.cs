using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using JobMasterApi.Dtos;
using ResumeUploadApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ SQL Server connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(connectionString));

// ✅ Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>();

// ✅ JWT Authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtConfig["Issuer"],
		ValidAudience = jwtConfig["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
	};
});

builder.Services.AddAuthorization();

// ✅ CORS (optional)
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
	});
});

// ✅ Controllers
builder.Services.AddControllers();

// ✅ Swagger + JWT Auth + Examples
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "JobMaster API", Version = "v1" });

	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "Enter 'Bearer' [space] and then your valid token.",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "bearer",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			Array.Empty<string>()
		}
	});

	options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<LoginDtoExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<RegisterDtoExample>();
builder.Services.AddSwaggerExamples();

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
