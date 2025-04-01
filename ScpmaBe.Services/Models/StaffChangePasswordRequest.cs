namespace ScpmaBe.Services.Models
{
    public class StaffChangePasswordRequest
    {
        public int StaffId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
