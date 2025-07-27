
namespace GamblersHell.Shared.Interface
{
    public interface IAuthRepository
    {
        public Task <UserDTO> GetUserAuthAsync (LoginDTO loginModel);
    }
}
