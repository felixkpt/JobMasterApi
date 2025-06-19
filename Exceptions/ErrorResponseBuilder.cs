using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobMasterApi.Exceptions
{
    public static class ErrorResponseBuilder
    {
        public static IActionResult FromIdentityErrors(
            IEnumerable<IdentityError> errors,
            HttpContext httpContext
        )
        {
            var formattedErrors = new Dictionary<string, string[]>();

            foreach (var error in errors)
            {
                var field = MapIdentityErrorCodeToField(error.Code);

                if (!formattedErrors.ContainsKey(field))
                    formattedErrors[field] = [];

                formattedErrors[field] = formattedErrors[field].Append(error.Description).ToArray();
            }

            var response = new
            {
                message = "One or more validation errors occurred.",
                status = StatusCodes.Status400BadRequest,
                errors = formattedErrors,
                traceId = httpContext.TraceIdentifier,
            };

            return new BadRequestObjectResult(response);
        }

        public static IActionResult Single(
            string field,
            string message,
            HttpContext httpContext,
            int statusCode = StatusCodes.Status400BadRequest
        )
        {
            var response = new
            {
                message = "A validation error occurred.",
                status = statusCode,
                errors = new Dictionary<string, string[]> { [field] = [message] },
                traceId = httpContext.TraceIdentifier,
            };

            return new ObjectResult(response) { StatusCode = statusCode };
        }

        public static IActionResult FromStrings(
            string field,
            IEnumerable<string> errors,
            HttpContext httpContext,
            int statusCode = StatusCodes.Status400BadRequest
        )
        {
            var response = new
            {
                message = "One or more validation errors occurred.",
                status = statusCode,
                errors = new Dictionary<string, string[]> { [field] = errors.ToArray() },
                traceId = httpContext.TraceIdentifier,
            };

            return new ObjectResult(response) { StatusCode = statusCode };
        }

        private static string MapIdentityErrorCodeToField(string code)
        {
            return code switch
            {
                "DuplicateUserName" => "email",
                "InvalidEmail" => "email",
                "PasswordTooShort" => "password",
                "PasswordRequiresNonAlphanumeric" => "password",
                "PasswordRequiresDigit" => "password",
                "PasswordRequiresUpper" => "password",
                "PasswordRequiresLower" => "password",
                _ => "general",
            };
        }
    }
}
