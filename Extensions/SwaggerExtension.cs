
using JobMasterApi.Dtos;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace JobMasterApi.Extensions
{
    public static class SwaggerExtension
    {
        public static WebApplicationBuilder AddSwaggerConfigs(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo { Title = "JobMaster API", Version = "v1" }
                );

                // 🔐 JWT Auth Configuration
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter JWT token in the format: Bearer {your token}",
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            Array.Empty<string>()
                        },
                    }
                );

                options.ExampleFilters();
            });

            builder.Services.AddSwaggerExamplesFromAssemblyOf<LoginDtoExample>();
            builder.Services.AddSwaggerExamplesFromAssemblyOf<RegisterDtoExample>();
            builder.Services.AddSwaggerExamples();

            return builder;
        }
    }
}
