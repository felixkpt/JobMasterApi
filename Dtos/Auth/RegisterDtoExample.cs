using Swashbuckle.AspNetCore.Filters;

namespace JobMasterApi.Dtos.Auth
{
    public class RegisterDtoExample : IExamplesProvider<RegisterDto>
    {
        public RegisterDto GetExamples()
        {
            return new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@example.com",
                Password = "Demo@123",
            };
        }
    }
}
