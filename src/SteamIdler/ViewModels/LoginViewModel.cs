using Prism.Commands;
using Prism.Events;
using SteamIdler.Events;
using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Constants;
using SteamKit2;
using System;
using System.Threading;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private SteamBot _steamBot;
        private string _username;
        private string _errorText;
        private bool _isTryingToLogin;
        private CodeType? _codeType;
        private string _code;

        public LoginViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

            SignInCommand = new DelegateCommand(SignIn);

            Initialize();
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

        private void Initialize()
        {
            _steamBot = new SteamBot();
        }

        private async void SignIn()
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
                var loginResult = await _steamBot.LoginAsync(Username, Code, CodeType ?? null, tcs.Token);
                switch (loginResult.Result)
                {
                    case EResult.OK:
                        _eventAggregator.GetEvent<LoginSuccessfulEvent>().Publish(_steamBot);
                        break;
                    case EResult.AccountLogonDenied:
                    case EResult.AccountLogonDeniedVerifiedEmailRequired:
                        await _steamBot.AwaitDisconnectAsync();
                        CodeType = Infrastructure.Constants.CodeType.Auth;
                        await _steamBot.ConnectAsync();
                        break;
                    case EResult.AccountLoginDeniedNeedTwoFactor:
                        await _steamBot.AwaitDisconnectAsync();
                        CodeType = Infrastructure.Constants.CodeType.TwoFactor;
                        await _steamBot.ConnectAsync();
                        break;
                    default:
                        await _steamBot.AwaitDisconnectAsync();
                        ErrorText = loginResult.Result.ToString();
                        await _steamBot.ConnectAsync();
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
