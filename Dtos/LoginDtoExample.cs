
using Swashbuckle.AspNetCore.Filters;

namespace JobMasterApi.Dtos
{
    public class LoginDtoExample : IExamplesProvider<LoginDto>
    {
        public LoginDto GetExamples()
        {
            return new LoginDto
            {
                Email = "johndoe@example.com",
                Password = "Demo@123"
            };
        }
    }
}
