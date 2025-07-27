using System.ComponentModel.DataAnnotations;

namespace GamblersHell.Shared
{
    public class UserChangePasswordDTO
    {
        public string currentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(20, ErrorMessage = "Password cannot be longer than 20 characters.")]
        public string newPassword { get; set; } = string.Empty;
    }
}
