﻿namespace ScpmaBe.Services.Models
{
    public class CustomerResponse
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsActive  { get; set; }
    }
}
