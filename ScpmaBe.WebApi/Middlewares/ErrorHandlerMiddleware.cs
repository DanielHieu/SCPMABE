using Azure;
using ScpmBe.Services.Exceptions;
using System.Net;
using System.Text.Json;

namespace ScpmBe.WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException error)
            {
                HandleException(context, error);

                var response = context.Response;

                var result = JsonSerializer.Serialize(new { msgId = error?.MessageId, message = error?.Message });

                await response.WriteAsync(result);

            }
            catch (Exception error)
            {
                HandleException(context, error);

                var response = context.Response;
                
                var result = JsonSerializer.Serialize(new { message = error?.Message });
                
                await response.WriteAsync(result);
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
