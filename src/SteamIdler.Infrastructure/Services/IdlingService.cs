using SteamIdler.Infrastructure.Exceptions;
using SteamIdler.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Services
{
    public class IdlingService
    {
        private static IdlingService _instance;
        public static IdlingService Instance
        {
            get => _instance;
        }

        public static IdlingService FromAccounts(IEnumerable<SteamAccount> accounts)
        {
            if (_instance != null)
            {
                return _instance;
            }

            var service = new IdlingService(accounts);
            _instance = service;

            return service;
        }

        private readonly Repository<Account, int> _accountRepository;
        private readonly List<SteamAccount> _steamAccounts;

        private IdlingService(IEnumerable<SteamAccount> accounts)
        {
            _accountRepository = new Repository<Account, int>();
            _steamAccounts = new List<SteamAccount>(accounts ?? throw new ArgumentNullException(nameof(accounts)));
        }

        public IEnumerable<SteamAccount> SteamAccounts
        {
            get => _steamAccounts;
        }

        public bool AllBotsAreRunning
        {
            get => _steamAccounts.All(a => a.SteamBot.IsRunning);
        }

        public bool AllBotsAreIdling
        {
            get => _steamAccounts.All(a => a.SteamBot.IsRunningApp);
        }

        public async void Initialize()
        {
            _steamAccounts.Clear();

            var accounts = await _accountRepository.GetAllItemsAsync();

            foreach (var account in accounts)
            {
                var steamAccount = new SteamAccount
                {
                    Account = account,
                    SteamBot = new SteamBot()
                };
                _steamAccounts.Add(steamAccount);
            }
        }

        public void AddAccount(SteamAccount account, bool startIdling = false, CancellationToken cancellationToken = default)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            _steamAccounts.Add(account);

            if (startIdling)
            {
                StartIdling(account.SteamBot);
            }
        }

        public async Task RemoveAccountAsync(SteamAccount account, bool logout = true, CancellationToken cancellationToken = default)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (logout && account.SteamBot.IsConnected)
            {
                await account.SteamBot.LogoutAsync(cancellationToken);
            }

            _steamAccounts.Remove(account);
        }

        public void StartIdling()
        {
            foreach (var bot in _steamAccounts.Select(t => t.SteamBot))
            {
                StartIdling(bot);
            }
        }

        public void StartIdling(string username, IEnumerable<App> apps = null)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            var bot = _steamAccounts
                .Select(t => t.SteamBot)
                .FirstOrDefault(b => b.LogOnDetails.Username.Equals(username));

            if (bot == null)
            {
                throw new BotNotFoundException(username);
            }

            StartIdling(bot, apps);
        }

        public async void StartIdling(SteamBot bot, IEnumerable<App> apps = null)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            var account = await _accountRepository.GetFirstItemAsync(a => a.Username.Equals(bot.LogOnDetails.Username));
            if (account == null)
            {
                throw new AccountNotFoundException(bot.LogOnDetails.Username);
            }

            if (apps == null)
            {
                apps = account.AccountApps.Select(aa => aa.App);
            }

            // TODO: 아이들링 관련 코드 작성하면 됨
        }
    }
}
