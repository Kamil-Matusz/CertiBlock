using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CertiBlock.Shared.Exceptions;

internal sealed class ErrorHandlerMiddleware(
    ILogger<ErrorHandlerMiddleware> logger,
    IExceptionCompositionRoot exceptionCompositionRoot) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var errorResponse = exceptionCompositionRoot.Map(exception);
        context.Response.StatusCode = (int)(errorResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        var response = errorResponse?.Response;
        if (response is null)
        {
            return;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}