using System.ComponentModel.DataAnnotations;

namespace ScpmaBe.Services.Models
{
    public partial class LoginOwnerRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
