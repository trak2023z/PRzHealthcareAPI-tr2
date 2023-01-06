using System.Diagnostics;

namespace PRzHealthcareAPI.Middlewares
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private Stopwatch _stopWatch;

        public RequestTimeMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            this._logger = logger;
            _stopWatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopWatch.Start();
            await next.Invoke(context);
            _stopWatch.Stop();

            var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
            if (elapsedMilliseconds / 1000 > 4)
            {
                var message = $@"Request [{context.Request.Method}] at {context.Request.Path} took {elapsedMilliseconds} ms.";

                _logger.LogInformation(message);
            }
        }
    }
}
