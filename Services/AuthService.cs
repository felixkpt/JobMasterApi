using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JobMasterApi.Dtos.Auth;
using JobMasterApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResumeUploadApi.Data;
using ResumeUploadApi.Models;

namespace JobMasterApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _db;
        private readonly ITemplateService _templateService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailSender emailSender,
            AppDbContext db,
            ITemplateService templateService
        )
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _db = db;
            _templateService = templateService;
        }

        public async Task<(
            bool Success,
            string? Token,
            IEnumerable<IdentityError>? Errors
        )> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return (false, null, result.Errors);
            }

            var token = GenerateJwtToken(user);
            return (true, token, null);
        }

        public async Task<(bool Success, string? Token)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                var token = GenerateJwtToken(user);
                return (true, token);
            }

            return (false, null);
        }

        public async Task<bool> SendResetPasswordEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || string.IsNullOrEmpty(user.Email))
                return false;

            // Invalidate old tokens
            var existing = await _db
                .PasswordResets.Where(p => p.UserId == user.Id && !p.Used)
                .ToListAsync();

            _db.PasswordResets.RemoveRange(existing);

            // Generate a token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            var reset = new PasswordReset
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = expiresAt,
            };

            _db.PasswordResets.Add(reset);
            await _db.SaveChangesAsync();

            var numericCode = GenerateNumericCode();

            var callbackUrl =
                $"{_configuration["App:FrontendUrl"]}/reset-password?token={WebUtility.UrlEncode(token)}";

            var resetTokenExpiresIn = _configuration["Auth:ResetTokenExpiresIn"]!;
            var resetTokenExpiresInHours = int.Parse(resetTokenExpiresIn) / 60;
            string resetTokenExpiresInDisplay;
            if (resetTokenExpiresInHours > 1)
            {
                resetTokenExpiresInDisplay = $"{resetTokenExpiresInHours} hours";
            }
            else
            {
                resetTokenExpiresInDisplay = $"{resetTokenExpiresInHours} hour";
            }

            var replacements = new Dictionary<string, string>
            {
                { "FirstName", user.FirstName },
                { "ResetCode", numericCode },
                { "ResetLink", callbackUrl },
                { "SupportEmail", _configuration["App:SupportEmail"]! },
                { "CompanyName", _configuration["App:CompanyName"]! },
                { "ResetTokenExpiresIn", resetTokenExpiresInDisplay },
            };

            var body = await _templateService.RenderTemplateAsync(
                "PasswordResetTemplate.html",
                replacements
            );

            await _emailSender.SendEmailAsync(user.Email, "Password Reset Request", body);
            return true;
        }

        public async Task<bool> VerifyResetTokenAsync(string token)
        {
            var reset = await _db
                .PasswordResets.Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Token == token && r.ExpiresAt > DateTime.UtcNow && !r.Used
                );

            return reset != null;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var reset = await _db
                .PasswordResets.Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Token == dto.Token && r.ExpiresAt > DateTime.UtcNow && !r.Used
                );

            if (reset == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(reset.User, dto.Token, dto.Password);

            if (!result.Succeeded)
                return false;

            reset.Used = true;
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<(bool Success, IEnumerable<string>? Errors)> UpdatePasswordAsync(
            string userId,
            UpdatePasswordDto dto
        )
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, new[] { "User not found." });

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            return (true, null);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Auth:Jwt");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
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

        private string GenerateNumericCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
