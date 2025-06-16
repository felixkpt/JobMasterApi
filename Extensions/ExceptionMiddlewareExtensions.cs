using System.Net;
using JobMasterApi.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace JobMasterApi.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var response = context.Response;
                response.ContentType = "application/json";

                var statusCode = exception is AppException apiEx ? apiEx.StatusCode : (int)HttpStatusCode.InternalServerError;
                response.StatusCode = statusCode;

                var result = new
                {
                    message = exception?.Message ?? "An unexpected error occurred.",
                    status = statusCode,
                    traceId = context.TraceIdentifier
                };

                await context.Response.WriteAsJsonAsync(result);
            });
        });
    }
}
