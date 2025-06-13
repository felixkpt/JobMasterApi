using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ResumeUploadApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        protected BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<string> GetUserIdAsync()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("User email not found in token.");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            return user.Id;
        }
    }
}
