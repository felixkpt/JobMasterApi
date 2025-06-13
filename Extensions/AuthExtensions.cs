using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace JobMasterApi.Extensions
{
	public static class AuthExtensions
	{
		public static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
		{


            var jwtConfig = builder.Configuration.GetSection("Jwt");
			builder
				.Services.AddAuthentication(options =>
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
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(jwtConfig["Key"]!)
						),
					};
				});
			return builder;
		}
	}
}
