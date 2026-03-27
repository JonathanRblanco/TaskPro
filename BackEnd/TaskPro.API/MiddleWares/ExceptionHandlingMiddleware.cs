using System.Text.Json;
using TaskPro.Domain.Exceptions;

namespace TaskPro.API.MiddleWares
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (UnauthorizedDomainException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status403Forbidden, ex.Message);
            }
            catch (ConflictException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (DomainException ex)
            {
                await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unhandled exception on {context.Request.Method} {context.Request.Path}");

                await WriteErrorAsync(context,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(body);
        }
    }
}
