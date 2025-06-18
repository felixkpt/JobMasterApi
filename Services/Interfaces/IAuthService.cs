using JobMasterApi.Dtos.Auth;
using Microsoft.AspNetCore.Identity;

namespace JobMasterApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token, IEnumerable<IdentityError>? Errors)> RegisterAsync(
            RegisterDto dto
        );
        Task<(bool Success, string? Token)> LoginAsync(LoginDto dto);
        Task<bool> SendResetPasswordEmailAsync(string email);
        Task<bool> VerifyResetTokenAsync(string token);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
