using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ResumeUploadApi.Exceptions
{
    public static class CustomProblemDetailsFactory
    {
        public static IActionResult Create(ModelStateDictionary modelState, HttpContext httpContext)
        {
            var errors = modelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
                );

            var result = new
            {
                message = "One or more validation errors occurred.",
                status = StatusCodes.Status400BadRequest,
                errors,
                traceId = httpContext.TraceIdentifier
            };

            return new BadRequestObjectResult(result);
        }
    }
}
