namespace ScpmaBe.Services.Models
{
    public class ChangePasswordRequest
    {
        public int CustomerId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
