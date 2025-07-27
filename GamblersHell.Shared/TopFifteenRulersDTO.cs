
namespace GamblersHell.Shared
{
    public class TopFifteenRulersDTO
    {
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int Balance { get; set; }

        public int BalanceAfterWin { get; set; }
    }
}
