using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Repositories;
using SteamIdler.Views;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Services
{
    public interface IAccountService
    {
        Task<SteamBot> AddAccountAsync(CancellationToken cancellationToken = default);
    }

    public class AccountService : IAccountService
    {
        private readonly IRepository<Account, int> _accountRepository;

        public AccountService()
        {
            _accountRepository = new AccountRepository();
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
                Password = bot.LogOnDetails.Password,
                AutomaticLogin = bot.LogOnDetails.ShouldRememberPassword
            };
            await _accountRepository.AddAsync(account, cancellationToken);

            var dbAccount = await _accountRepository.GetFirstItemAsync(a => a.Username.Equals(account), cancellationToken);
            if (!string.IsNullOrWhiteSpace(bot.LogOnDetails.LoginKey))
            {
                dbAccount.LoginKey = bot.LogOnDetails.LoginKey;
                await _accountRepository.EditAsync(dbAccount, cancellationToken);
            }

            bot.Account = dbAccount;

            return bot;
        }
    }
}
