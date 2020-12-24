using SteamBot;
using SteamIdler.Infrastructure.Constants;
using SteamIdler.Infrastructure.Helpers;
using SteamKit2;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Services
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

        public BotService()
        {
            _passwordService = PasswordService.Instance;
        }

        public async Task<SteamClient.ConnectedCallback> ConnectAsync(Bot bot, CancellationToken cancellationToken = default)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            var result = await TaskExt
                .FromEvent<SteamClient.ConnectedCallback>()
                .Start(handler => bot.Connected += handler,
                       () => bot.ConnectAndWaitCallbacks(),
                       handler => bot.Connected -= handler,
                       cancellationToken);

            return result;
        }

        public async Task<SteamClient.DisconnectedCallback> DisconnectAsync(Bot bot, CancellationToken cancellationToken = default)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            var result = await TaskExt
                .FromEvent<SteamClient.DisconnectedCallback>()
                .Start(handler => bot.Disconnected += handler,
                       () => bot.Disconnect(),
                       handler => bot.Disconnected -= handler,
                       cancellationToken);

            return result;
        }

        public async Task<SteamClient.ConnectedCallback> ReconnectAsync(Bot bot, CancellationToken cancellationToken = default)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            if (bot.IsConnected)
            {
                await DisconnectAsync(bot, cancellationToken);
            }
            var result = await ConnectAsync(bot, cancellationToken);

            return result;
        }

        public async Task<SteamUser.LoggedOnCallback> LoginAsync(Bot bot, string username, string code = null, CodeType? codeType = null, CancellationToken cancellationToken = default)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            SteamUser.LoggedOnCallback result;

            try
            {
                bot.LogOnDetails.Username = username;
                if (_passwordService.GetPassword == null)
                {
                    throw new Exception("GetPassword function not found.");
                }
                bot.LogOnDetails.Password = _passwordService.GetPassword();
                if (codeType.HasValue)
                {
                    switch (codeType)
                    {
                        case CodeType.Auth:
                            bot.LogOnDetails.AuthCode = code;
                            break;
                        case CodeType.TwoFactor:
                            bot.LogOnDetails.TwoFactorCode = code;
                            break;
                    }
                }

                result = await TaskExt
                    .FromEvent<SteamUser.LoggedOnCallback>()
                    .Start(handler => bot.LoggedOn += handler,
                           () => bot.Login(),
                           handler => bot.LoggedOn -= handler,
                           cancellationToken);
            }
            finally
            {
                bot.LogOnDetails.Username = null;
                bot.LogOnDetails.Password = null;
                bot.LogOnDetails.AuthCode = null;
                bot.LogOnDetails.TwoFactorCode = null;
            }

            return result;
        }

        public async Task<SteamClient.DisconnectedCallback> AwaitDisconnectAsync(Bot bot, CancellationToken cancellationToken = default)
        {
            if (bot == null)
            {
                throw new ArgumentNullException(nameof(bot));
            }

            var result = await TaskExt
                .FromEvent<SteamClient.DisconnectedCallback>()
                .Start(handler => bot.Disconnected += handler,
                       () => { },
                       handler => bot.Disconnected -= handler,
                       cancellationToken);

            return result;
        }
    }
}
