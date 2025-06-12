using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobMasterApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace JobMasterApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IConfiguration _configuration;

		public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		[SwaggerRequestExample(typeof(RegisterDto), typeof(Dtos.RegisterDtoExample))]
		[SwaggerOperation(Summary = "Register user and return JWT token")]
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
			var result = await _userManager.CreateAsync(user, dto.Password);

			if (!result.Succeeded)
				return BadRequest(result.Errors);

			// Generate token for newly registered user
			var token = GenerateJwtToken(user);
			return Ok(new { token });
		}

		[AllowAnonymous]
		[HttpPost("login")]
		[SwaggerRequestExample(typeof(LoginDto), typeof(Dtos.LoginDtoExample))]
		[SwaggerOperation(Summary = "Authenticate user and return JWT token")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
			{
				var token = GenerateJwtToken(user);
				return Ok(new { token });
			}

			return Unauthorized();
		}

		private string GenerateJwtToken(IdentityUser user)
		{
			var jwtSettings = _configuration.GetSection("Jwt");
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"]!)),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

	}
