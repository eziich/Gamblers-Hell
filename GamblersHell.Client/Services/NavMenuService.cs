namespace GamblersHell.Client.Services
{
    public class NavMenuState
    {
        public event Action OnNavMenuRefresh;

        public void RefreshNavMenu()
        {
            OnNavMenuRefresh?.Invoke();
        }
    }
}
