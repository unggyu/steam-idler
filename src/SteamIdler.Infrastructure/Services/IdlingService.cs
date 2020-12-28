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

        public static IdlingService FromAccounts(IEnumerable<SteamBot> accounts)
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
        private readonly List<SteamBot> _steamBots;

        private IdlingService(IEnumerable<SteamBot> bots)
        {
            _accountRepository = new Repository<Account, int>();
            _steamBots = new List<SteamBot>(bots ?? throw new ArgumentNullException(nameof(bots)));
        }

        public IEnumerable<SteamBot> SteamAccounts
        {
            get => _steamBots;
        }

        public bool AllBotsAreIdling
        {
            get => _steamBots.All(a => a.IsRunningApp);
        }

        public void AddBot(SteamBot steamBot, bool startIdling = false, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            _steamBots.Add(steamBot);

            if (startIdling)
            {
                StartIdling(steamBot);
            }
        }

        public async Task RemoveAccountAsync(SteamBot steamBot, bool logout = true, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            if (logout && steamBot.IsConnected)
            {
                await steamBot.LogoutAsync(cancellationToken);
            }

            _steamBots.Remove(steamBot);
        }

        public void StartIdling()
        {
            foreach (var bot in _steamBots)
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

            var bot = _steamBots
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
