using Swashbuckle.AspNetCore.Filters;
using JobMasterApi.Dtos;

namespace JobMasterApi.Dtos
{
    public class RegisterDtoExample : IExamplesProvider<RegisterDto>
    {
        public RegisterDto GetExamples()
        {
            return new RegisterDto
            {
                Email = "user@example.com",
                Password = "Demo@123"
            };
        }
    }
}
