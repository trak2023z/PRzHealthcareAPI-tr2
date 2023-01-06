using PRzHealthcareAPI.Exceptions;
using System.Text.Json;

namespace PRzHealthcareAPI.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            this._logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException badRequestException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                var result = JsonSerializer.Serialize(new { status = context.Response.StatusCode, message = badRequestException?.Message });
                await context.Response.WriteAsync(result);
            }
            catch (NotFoundException notFoundException)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 404;
                var result = JsonSerializer.Serialize(new { status = context.Response.StatusCode, message = notFoundException?.Message });
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                var result = JsonSerializer.Serialize(new { status = context.Response.StatusCode, message = ex?.Message });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
