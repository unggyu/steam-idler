using SteamBot;
using SteamIdler.Constants;
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

        public async Task<SteamClient.ConnectedCallback> ConnectAsync(CancellationToken cancellationToken = default)
        {
            var result = await TaskExt
                .FromEvent<SteamClient.ConnectedCallback>()
                .Start(handler => _bot.Connected += handler,
                       () => _bot.ConnectAndWaitCallbacks(),
                       handler => _bot.Connected -= handler,
                       cancellationToken);

            return result;
        }

        public async Task<SteamClient.DisconnectedCallback> DisconnectAsync(CancellationToken cancellationToken = default)
        {
            var result = await TaskExt
                .FromEvent<SteamClient.DisconnectedCallback>()
                .Start(handler => _bot.Disconnected += handler,
                       () => _bot.Disconnect(),
                       handler => _bot.Disconnected -= handler,
                       cancellationToken);

            return result;
        }

        public async Task<SteamClient.ConnectedCallback> ReconnectAsync(CancellationToken cancellationToken = default)
        {
            if (_bot.IsConnected)
            {
                await DisconnectAsync(cancellationToken);
            }
            var result = await ConnectAsync(cancellationToken);

            return result;
        }

        public async Task<SteamUser.LoggedOnCallback> LoginAsync(string username, string code = null, CodeType? codeType = null, CancellationToken cancellationToken = default)
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
                if (codeType.HasValue)
                {
                    switch (codeType)
                    {
                        case CodeType.Auth:
                            _bot.LogOnDetails.AuthCode = code;
                            break;
                        case CodeType.TwoFactor:
                            _bot.LogOnDetails.TwoFactorCode = code;
                            break;
                    }
                }

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

        public async Task<SteamClient.DisconnectedCallback> AwaitDisconnectAsync(CancellationToken cancellationToken = default)
        {
            var result = await TaskExt
                .FromEvent<SteamClient.DisconnectedCallback>()
                .Start(handler => _bot.Disconnected += handler,
                       () => { },
                       handler => _bot.Disconnected -= handler,
                       cancellationToken);

            return result;
        }
    }
}
