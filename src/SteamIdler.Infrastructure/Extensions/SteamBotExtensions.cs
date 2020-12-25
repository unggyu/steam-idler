using SteamIdler.Infrastructure.Constants;
using SteamIdler.Infrastructure.Helpers;
using SteamIdler.Infrastructure.Services;
using SteamKit2;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure
{
    public static class SteamBotExtensions
    {
        private static readonly PasswordProvider _passwordProvider = PasswordProvider.Instance;

        public static async Task<SteamClient.ConnectedCallback> ConnectAsync(this SteamBot steamBot, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            var result = await TaskExt
                .FromEvent<SteamClient.ConnectedCallback>()
                .Start(handler => steamBot.Connected += handler,
                       () => steamBot.ConnectAndWaitCallbacks(),
                       handler => steamBot.Connected -= handler,
                       cancellationToken);

            return result;
        }

        public static async Task<SteamClient.DisconnectedCallback> DisconnectAsync(this SteamBot steamBot, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            var result = await TaskExt
                .FromEvent<SteamClient.DisconnectedCallback>()
                .Start(handler => steamBot.Disconnected += handler,
                       () => steamBot.Disconnect(),
                       handler => steamBot.Disconnected -= handler,
                       cancellationToken);

            return result;
        }

        public static async Task<SteamClient.ConnectedCallback> ReconnectAsync(this SteamBot steamBot, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            if (steamBot.IsConnected)
            {
                await steamBot.DisconnectAsync(cancellationToken);
            }
            var result = await steamBot.ConnectAsync(cancellationToken);

            return result;
        }

        public static async Task<SteamUser.LoggedOnCallback> LoginAsync(this SteamBot steamBot, string username, string code = null, CodeType? codeType = null, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            if (!steamBot.IsConnected)
            {
                await steamBot.ConnectAsync(cancellationToken);
            }

            steamBot.LogOnDetails.Username = username;
            if (_passwordProvider.GetPassword == null)
            {
                throw new Exception("GetPassword function not found.");
            }

            steamBot.LogOnDetails.Password = _passwordProvider.GetPassword();
            if (codeType.HasValue)
            {
                switch (codeType)
                {
                    case CodeType.Auth:
                        steamBot.LogOnDetails.AuthCode = code;
                        break;
                    case CodeType.TwoFactor:
                        steamBot.LogOnDetails.TwoFactorCode = code;
                        break;
                }
            }

            var result = await TaskExt
                .FromEvent<SteamUser.LoggedOnCallback>()
                .Start(handler => steamBot.LoggedOn += handler,
                       () => steamBot.Login(),
                       handler => steamBot.LoggedOn -= handler,
                       cancellationToken);

            return result;
        }

        public static async Task<SteamClient.DisconnectedCallback> AwaitDisconnectAsync(this SteamBot steamBot, CancellationToken cancellationToken = default)
        {
            if (steamBot == null)
            {
                throw new ArgumentNullException(nameof(steamBot));
            }

            var result = await TaskExt
                .FromEvent<SteamClient.DisconnectedCallback>()
                .Start(handler => steamBot.Disconnected += handler,
                       () => { },
                       handler => steamBot.Disconnected -= handler,
                       cancellationToken);

            return result;
        }
    }
}
