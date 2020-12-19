using SteamBot;
using SteamIdler.Helpers;
using SteamKit2;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Services
{
    public class BotService
    {
        private static BotService _instance;
        private readonly PasswordService _passwordService;
        private readonly Bot _bot;

        public static BotService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BotService();
                }

                return _instance;
            }
        }

        public BotService()
        {
            _passwordService = PasswordService.Instance;
            _bot = new Bot();
        }

        public async Task<SteamClient.ConnectedCallback> ConnectAsync()
        {
            var result = await TaskExt
                .FromEvent<SteamClient.ConnectedCallback>()
                .Start(handler => _bot.Connected += handler,
                       async () => await _bot.ConnectAndWaitCallbacksAsync(),
                       handler => _bot.Connected -= handler);

            return result;
        }

        public async Task<SteamUser.LoggedOnCallback> LoginAsync(string username, CancellationToken cancellationToken = default)
        {
            _bot.LogOnDetails.Username = username;
            if (_passwordService.GetPassword == null)
            {
                throw new Exception("GetPassword function not found.");
            }
            _bot.LogOnDetails.Password = _passwordService.GetPassword();

            var result = await TaskExt
                .FromEvent<SteamUser.LoggedOnCallback>()
                .Start(handler => _bot.LoggedOn += handler,
                       () => _bot.Login(),
                       handler => _bot.LoggedOn -= handler,
                       cancellationToken);

            return result;
        }
    }
}
