namespace ScpmaBe.Services.Models
{
    public class UpdateCustomerRequest
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }
    }
}
