﻿namespace ScpmaBe.Services.Models
{
    public class UpdateStaffRequest
    {
        public int StaffAccountId { get; set; }
        public int OwnerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsActived { get; set; }
    }
}
