namespace JobMasterApi.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
