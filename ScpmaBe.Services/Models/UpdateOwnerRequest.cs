namespace ScpmaBe.Services.Models
{
    public class UpdateOwnerRequest
    {
        public int OwnerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
