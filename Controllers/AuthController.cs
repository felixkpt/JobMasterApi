using JobMasterApi.Dtos.Auth;
using JobMasterApi.Exceptions;
using JobMasterApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadApi.Controllers;
using ResumeUploadApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace JobMasterApi.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(UserManager<ApplicationUser> userManager, IAuthService authService)
        : base(userManager)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [SwaggerRequestExample(typeof(RegisterDto), typeof(RegisterDtoExample))]
    [SwaggerOperation(Summary = "Register user and return JWT token")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var (success, token, errors) = await _authService.RegisterAsync(dto);

        if (!success && errors != null)
            return ErrorResponseBuilder.FromIdentityErrors(errors, HttpContext);

        return Ok(new { token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerRequestExample(typeof(LoginDto), typeof(LoginDtoExample))]
    [SwaggerOperation(Summary = "Authenticate user and return JWT token")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var (success, token) = await _authService.LoginAsync(dto);

        if (!success)
        {
            return ErrorResponseBuilder.Single(
                "email",
                "Invalid email or password",
                HttpContext,
                StatusCodes.Status401Unauthorized
            );
        }

        return Ok(new { token });
    }

    [HttpPost("forgot-password")]
    [SwaggerRequestExample(typeof(ForgotPasswordDto), typeof(ForgotPasswordDtoExample))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var sent = await _authService.SendResetPasswordEmailAsync(dto.Email);
        if (!sent)
            throw new AppException("Email not found", 400);

        return Ok(new { message = "Reset email sent" });
    }

    [HttpPost("verify-reset-token")]
    public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenDto dto)
    {
        var isValid = await _authService.VerifyResetTokenAsync(dto.Token);
        if (!isValid)
            throw new AppException("Invalid or expired token", 400);

        return Ok(new { message = "Token is valid" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var reset = await _authService.ResetPasswordAsync(dto);
        if (!reset)
            throw new AppException("Reset failed", 400);

        return Ok(new { message = "Password successfully reset" });
    }

    [Authorize]
    [HttpPost("update-password")]
    [SwaggerOperation(Summary = "Update user password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        var userId = await GetUserIdAsync();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User not authenticated" });

        var (success, errors) = await _authService.UpdatePasswordAsync(userId, dto);

        if (!success && errors != null)
            return ErrorResponseBuilder.FromStrings("password", errors, HttpContext);

        return Ok(new { message = "Password updated successfully" });
    }
}
