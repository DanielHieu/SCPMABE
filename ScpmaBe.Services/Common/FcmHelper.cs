using System.Text;
using Microsoft.Extensions.Logging;

namespace ScpmaBe.Services.Common
{
    public interface IFcmSender
    {
        Task SendNotificationAsync(string serverKey, string deviceToken, string title, string body);
    }


    public class FcmSender : IFcmSender
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ILogger _logger;

        public FcmSender(ILogger<FcmSender> logger)
        {
            _logger = logger;
        }

        public async Task SendNotificationAsync(string serverKey, string deviceToken, string title, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(serverKey))
                {
                    _logger.LogError("Server key is null or empty.");
                    return;
                }
                if (string.IsNullOrEmpty(deviceToken))
                {
                    _logger.LogError("Device token is null or empty.");
                    return;
                }
                if (string.IsNullOrEmpty(title))
                {
                    _logger.LogError("Title is null or empty.");
                    return;
                }
                if (string.IsNullOrEmpty(body))
                {
                    _logger.LogError("Body is null or empty.");
                    return;
                }

                var requestUri = "https://fcm.googleapis.com/fcm/send";

                var message = new
                {
                    to = deviceToken,
                    notification = new
                    {
                        title,
                        body
                    },
                    priority = "high"
                };

                var jsonMessage = System.Text.Json.JsonSerializer.Serialize(message);

                var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

                request.Headers.TryAddWithoutValidation("Authorization", $"key={serverKey}");
                request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"FCM Response: {response.StatusCode} - {responseContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendNotificationAsync");
            }
        }
    }
}
