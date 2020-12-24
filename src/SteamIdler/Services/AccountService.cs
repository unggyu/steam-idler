using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using SteamIdler.Views;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Services
{
    public class AccountService
    {
        private static AccountService _instance;

        public static AccountService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AccountService();
                }

                return _instance;
            }
        }

        private readonly IdlingService _idlingService;

        public AccountService()
        {
            _idlingService = IdlingService.Instance;
        }

        public async Task<Account> AddAccountAsync(CancellationToken cancellationToken = default)
        {
            var loginWindow = new LoginWindow();
            var result = loginWindow.ShowDialog();
            if (result == null || !result.Value)
            {
                return null;
            }

            var bot = loginWindow.Bot;
            _idlingService.AddBot(bot);
        }
    }
}
