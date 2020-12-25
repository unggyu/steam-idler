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
        private readonly List<(Account, SteamBot)> _accountBotTuples;

        public IdlingService()
        {
            _accountRepository = new Repository<Account, int>();
            _accountBotTuples = new List<(Account, SteamBot)>();
        }

        public IEnumerable<SteamBot> Bots
        {
            get => _accountBotTuples.Select(t => t.Item2);
        }

        public bool AllBotsAreRunning
        {
            get => _accountBotTuples.All(t => t.Item2.IsRunning);
        }

        public bool AllBotsAreIdling
        {
            get => _accountBotTuples.All(b => b.Item2.IsRunningApp);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _accountBotTuples.Clear();

            var accounts = await _accountRepository.GetAllItemsAsync(cancellationToken);

            foreach (var account in accounts)
            {
                var bot = new SteamBot(account);
                _accountBotTuples.Add((account, bot));
            }
        }

        public async Task AddBotAsync(SteamBot bot, bool startIdling = false, CancellationToken cancellationToken = default)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            var account = await _accountRepository.GetFirstItemAsync(a => a.Username.Equals(bot.LogOnDetails.Username), cancellationToken);
            if (account == null)
            {
                throw new AccountNotFoundException(bot.LogOnDetails.Username);
            }

            _accountBotTuples.Add((account, bot));

            if (startIdling)
            {
                StartIdling(bot);
            }
        }

        public void StartIdling()
        {
            foreach (var bot in _accountBotTuples.Select(t => t.Item2))
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

            var bot = _accountBotTuples
                .Select(t => t.Item2)
                .FirstOrDefault(b => b.LogOnDetails.Username.Equals(username));

            if (bot == null)
            {
                throw new BotNotFoundException();
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

        public SteamBot GetBotByAccount(Account account)
        {
            return _accountBotTuples.FirstOrDefault(t => t.Item1.Equals(account)).Item2;
        }

        public Account GetAccountByBot(SteamBot bot)
        {
            return _accountBotTuples.FirstOrDefault(t => t.Item2.Equals(bot)).Item1;
        }
    }
}
