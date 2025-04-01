namespace ScpmaBe.Services.Models
{
    public class StaffResponse
    {
        public int StaffId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public string FullName { get; set; }
        public bool IsActive { get; set; }
    }
}
