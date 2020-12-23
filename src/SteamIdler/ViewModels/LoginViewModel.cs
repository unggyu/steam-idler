using Prism.Commands;
using Prism.Events;
using SteamIdler.Constants;
using SteamIdler.Events;
using SteamIdler.Services;
using SteamKit2;
using System;
using System.Threading;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly BotService _botService;
        private string _username;
        private string _errorText;
        private bool _isTryingToLogin;
        private CodeType? _codeType;
        private string _code;

        public LoginViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _botService = BotService.Instance;

            SignInCommand = new DelegateCommand(SignIn);
        }

        public string Username
        {
            get => _username;
            set => SetValue(ref _username, value);
        }

        public string ErrorText
        {
            get => _errorText;
            set => SetValue(ref _errorText, value);
        }

        public bool IsTryingToLogin
        {
            get => _isTryingToLogin;
            set => SetValue(ref _isTryingToLogin, value);
        }

        public CodeType? CodeType
        {
            get => _codeType;
            set => SetValue(ref _codeType, value);
        }

        public string Code
        {
            get => _code;
            set => SetValue(ref _code, value);
        }

        public ICommand SignInCommand { get; }

        public async void SignIn()
        {
            if (IsTryingToLogin)
            {
                return;
            }

            IsTryingToLogin = true;

            try
            {
                using var tcs = new CancellationTokenSource();
                tcs.CancelAfter(10000);
                var loginResult = await _botService.LoginAsync(Username, Code, CodeType ?? null, tcs.Token);
                switch (loginResult.Result)
                {
                    case EResult.OK:
                        _eventAggregator.GetEvent<LoginSuccessfulEvent>().Publish();
                        break;
                    case EResult.AccountLogonDenied:
                    case EResult.AccountLogonDeniedVerifiedEmailRequired:
                        await _botService.AwaitDisconnectAsync();
                        CodeType = Constants.CodeType.Auth;
                        await _botService.ConnectAsync();
                        break;
                    case EResult.AccountLoginDeniedNeedTwoFactor:
                        await _botService.AwaitDisconnectAsync();
                        CodeType = Constants.CodeType.TwoFactor;
                        await _botService.ConnectAsync();
                        break;
                    default:
                        await _botService.AwaitDisconnectAsync();
                        ErrorText = loginResult.Result.ToString();
                        await _botService.ConnectAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
            finally
            {
                IsTryingToLogin = false;
            }
        }
    }
}
