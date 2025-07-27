using System.ComponentModel.DataAnnotations;

namespace GamblersHell.Shared
{
    public class UserDTO
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, ErrorMessage = "Username must be less than 20 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(15, ErrorMessage = "First name length can't be more than 20 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "Last name length can't be more than 20 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(20, ErrorMessage = "Password cannot be longer than 20 characters.")]
        public string Password { get; set; }

        public int Level { get; set; }

        public int Balance { get; set; }

        public int BalanceAfterWin { get; set; } = 0;

        public int Eye { get; set; } = 0;

        public DateTime LastLogin { get; set; }

        public int PokerGameControl { get; set; }

        public int PandoraGameControl { get; set; }

        public int UserVerified { get; set; } = 0;

        public DateTime UserRegistrationTime { get; set; }
    }
}
