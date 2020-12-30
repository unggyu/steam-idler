using SteamIdler.Properties;
using SteamKit2;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Services
{
    public interface ISteamErrorMessageService
    {
        Task<string> GetErrorMessageAsync(EResult eResult, CancellationToken cancellationToken = default);
    }

    public class SteamErrorMessageService : ISteamErrorMessageService
    {
        public async Task<string> GetErrorMessageAsync(EResult eResult, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;

            return eResult switch
            {
                EResult.AccountLogonDenied                      => Resources.Description_AccountLogonDenied,
                EResult.AccountLogonDeniedVerifiedEmailRequired => Resources.Description_AccountLogonDeniedVerifiedEmailRequired,
                EResult.AccountLoginDeniedNeedTwoFactor         => Resources.Description_AccountLoginDeniedNeedTwoFactor,
                EResult.InvalidPassword                         => Resources.Description_InvalidPassword,
                EResult.TwoFactorCodeMismatch                   => Resources.Description_TwoFactorCodeMismatch,
                EResult.InvalidLoginAuthCode                    => Resources.Description_InvalidLoginAuthCode,
                _                                               => eResult.ToString(),
            };
        }
    }
}
