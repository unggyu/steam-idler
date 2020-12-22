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

        private readonly PasswordService _passwordService;
        private readonly Bot _bot;

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

        public async Task<SteamUser.LoggedOnCallback> LoginAsync(string username, string authCode = null, string twoFactorCode = null, CancellationToken cancellationToken = default)
        {
            SteamUser.LoggedOnCallback result;

            try
            {
                _bot.LogOnDetails.Username = username;
                if (_passwordService.GetPassword == null)
                {
                    throw new Exception("GetPassword function not found.");
                }
                _bot.LogOnDetails.Password = _passwordService.GetPassword();
                _bot.LogOnDetails.AuthCode = authCode;
                _bot.LogOnDetails.TwoFactorCode = twoFactorCode;

                result = await TaskExt
                    .FromEvent<SteamUser.LoggedOnCallback>()
                    .Start(handler => _bot.LoggedOn += handler,
                           () => _bot.Login(),
                           handler => _bot.LoggedOn -= handler,
                           cancellationToken);
            }
            finally
            {
                _bot.LogOnDetails.Username = null;
                _bot.LogOnDetails.Password = null;
                _bot.LogOnDetails.AuthCode = null;
                _bot.LogOnDetails.TwoFactorCode = null;
            }

            return result;
        }
    }
}
