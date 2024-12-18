using System.ComponentModel.DataAnnotations;

namespace FortniteAPI.Controllers
{
    public class RegistrationRequest
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;

        public string Role { get; set; } = "regular";
    }
}
