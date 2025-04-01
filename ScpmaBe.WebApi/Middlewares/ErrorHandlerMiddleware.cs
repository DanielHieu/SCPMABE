using ScpmBe.Services.Exceptions;
using System.Net;
using System.Text.Json;

namespace ScpmBe.WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException error)
            {
                LogException(context, error);
                HandleException(context, error);

                var response = context.Response;
                var result = JsonSerializer.Serialize(new { msgId = error?.MessageId, message = error?.Message });
                await response.WriteAsync(result);
            }
            catch (Exception error)
            {
                LogException(context, error);
                HandleException(context, error);

                var response = context.Response;
                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }

        private void LogException(HttpContext context, Exception error)
        {
            var requestPath = context.Request.Path;

            switch (error)
            {
                case BadRequestException e:
                    _logger.LogWarning(e, "Bad request error occurred for request {Path}. Message: {Message}", requestPath, e.Message);
                    break;
                case NotFoundException e:
                    _logger.LogWarning(e, "Resource not found for request {Path}. Message: {Message}", requestPath, e.Message);
                    break;
                case ConflictException e:
                    _logger.LogWarning(e, "Conflict error for request {Path}. Message: {Message}", requestPath, e.Message);
                    break;
                case AppException e:
                    _logger.LogError(e, "Application error for request {Path}. MessageId: {MessageId}, Message: {Message}",
                        requestPath, e.MessageId, e.Message);
                    break;
                default:
                    _logger.LogError(error, "Unhandled exception for request {Path}", requestPath);
                    break;
            }
        }

        private void HandleException(HttpContext context, Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case BadRequestException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ConflictException e:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }
        }
    }
}
