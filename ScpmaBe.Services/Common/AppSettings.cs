namespace ScpmaBe.Services.Common
{
    public class AppSettings
    {
        public string ApplicationUrl { get; set; }
        public string LandingPageUrl { get; set; }
       
        public PaymentSettings PaymentSettings { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}
