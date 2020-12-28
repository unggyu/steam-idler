using SteamIdler.Infrastructure;
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

        private readonly Repository<Account, int> _accountRepository;

        public AccountService()
        {
            _accountRepository = new Repository<Account, int>();
        }

        public async Task<SteamBot> AddAccountAsync(CancellationToken cancellationToken = default)
        {
            var loginWindow = new LoginWindow();
            var result = loginWindow.ShowDialog();
            if (result == null || !result.Value)
            {
                return null;
            }

            var bot = loginWindow.SteamBot;
            var account = new Account
            {
                Username = bot.LogOnDetails.Username,
                Password = bot.LogOnDetails.Password
            };
            await _accountRepository.AddAsync(account, cancellationToken);

            var dbAccount = await _accountRepository.GetFirstItemAsync(a => a.Username.Equals(account), cancellationToken);
            bot.Account = dbAccount;

            return bot;
        }
    }
}
