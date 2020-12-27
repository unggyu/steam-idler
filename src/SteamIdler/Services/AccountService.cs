using SteamIdler.Infrastructure.Exceptions;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using SteamIdler.Views;
using System;
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
        private readonly Repository<Account, int> _accountRepository;

        public AccountService()
        {
            _idlingService = IdlingService.Instance;
            _accountRepository = new Repository<Account, int>();
        }

        public async Task<SteamAccount> AddAccountAsync(CancellationToken cancellationToken = default)
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
            var steamAccount = new SteamAccount
            {
                Account = dbAccount,
                SteamBot = bot
            };
            _idlingService.AddAccount(steamAccount);

            return steamAccount;
        }

        public async Task RemoveAccountAsync(SteamAccount account, bool logout = true, CancellationToken cancellationToken = default)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            await _idlingService.RemoveAccountAsync(account, logout, cancellationToken);
            await _accountRepository.DeleteAsync(account.Account, cancellationToken);
        }
    }
}
