using Swashbuckle.AspNetCore.Filters;

namespace JobMasterApi.Dtos.Auth
{
    public class ForgotPasswordDtoExample : IExamplesProvider<ForgotPasswordDto>
    {
        public ForgotPasswordDto GetExamples()
        {
            return new ForgotPasswordDto { Email = "johndoe@example.com"};
        }
    }
}

