using System.Threading.Tasks;

namespace GamblersHell.Shared.Services
{
    public interface ITransactionService
    {
        Task<bool> LostToSatan(int userId);
    }
} 