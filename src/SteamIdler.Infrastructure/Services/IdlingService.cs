using SteamBot;
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
            get
            {
                if (_instance == null)
                {
                    _instance = new IdlingService();
                }

                return _instance;
            }
        }

        private readonly Repository<Account, int> _accountRepository;
        private List<Bot> _bots;

        public IdlingService()
        {
            _accountRepository = new Repository<Account, int>();
        }

        public IEnumerable<Bot> Bots
        {
            get => _bots;
        }

        public bool AllBotsAreRunning
        {
            get => _bots.All(b => b.IsRunning);
        }

        public bool AllBotsAreIdling
        {
            get => _bots.All(b => b.IsRunningApp);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var accounts = await _accountRepository.GetAllItemsAsync(cancellationToken);

            var bots = new List<Bot>();
            foreach (var account in accounts)
            {
                var bot = AccountToBot(account);
                bots.Add(bot);
            }

            _bots = bots;
        }

        public Bot AccountToBot(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            var bot = new Bot();
            bot.LogOnDetails.Username = account.Username;
            bot.LogOnDetails.Password = account.Password;

            return bot;
        }

        public void AddBot(Bot bot, bool startIdling = false)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            _bots.Add(bot);

            if (startIdling)
            {
                StartIdling(bot);
            }
        }

        public void StartIdling()
        {
            foreach (var bot in _bots)
            {
                StartIdling(bot);
            }
        }

        public void StartIdling(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            var bot = _bots.FirstOrDefault(b => b.LogOnDetails.Username.Equals(username));
            if (bot == null)
            {
                throw new BotNotFoundException();
            }

            StartIdling(bot);
        }

        public async void StartIdling(Bot bot)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            var account = await _accountRepository.GetFirstItemAsync(a => a.Username.Equals(bot.LogOnDetails.Username));
            if (account == null)
            {
                throw new AccountNotFoundException();
            }

            var apps = account.AccountApps.Select(aa => aa.App);

            // TODO: 아이들링 관련 코드 작성하면 됨
        }
    }
}
