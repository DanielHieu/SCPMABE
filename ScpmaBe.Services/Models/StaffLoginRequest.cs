using System.ComponentModel.DataAnnotations;

namespace ScpmaBe.Services.Models
{
    public partial class StaffLoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int ParkingLotId { get; set; }
    }
}
