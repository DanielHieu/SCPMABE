namespace ScpmaBe.Services.Models
{
    public class RegisterDeviceRequest
    {
        public int CustomerId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceToken { get; set; }
    }
}
