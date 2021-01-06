using Prism.Commands;
using Prism.Events;
using SteamIdler.Events;
using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Constants;
using SteamIdler.Properties;
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
        private readonly ISteamErrorMessageService _steamErrorMessageService;
        private SteamBot _steamBot;
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isTryingToLogin;
        private CodeType? _codeType;
        private string _code;
        private bool _rememberPassword;
        private bool _automaticLogin;

        private ICommand _signInCommand;

        public LoginViewModel(IEventAggregator eventAggregator, ISteamErrorMessageService steamErrorMessageService)
        {
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _steamErrorMessageService = steamErrorMessageService ?? throw new ArgumentNullException(nameof(steamErrorMessageService));

            Initialize();
        }

        public string Username
        {
            get => _username;
            set => SetValue(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetValue(ref _errorMessage, value);
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

        public bool RememberPassword
        {
            get => _rememberPassword;
            set => SetValue(ref _rememberPassword, value);
        }

        public bool AutomaticLogin
        {
            get => _automaticLogin;
            set => SetValue(ref _automaticLogin, value);
        }

        public ICommand SignInCommand
        {
            get => _signInCommand ??= new DelegateCommand(SignIn);
        }

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
                var loginResult = await _steamBot.LoginAsync(
                    username: Username,
                    password: Password,
                    code: Code,
                    codeType: CodeType ?? null,
                    cancellationToken: tcs.Token);

                switch (loginResult.Result)
                {
                    case EResult.OK:
                        ErrorMessage = null;
                        ErrorMessage = null;
                        Code = null;
                        CodeType = null;
                        _eventAggregator.GetEvent<LoginSuccessfulEvent>().Publish(_steamBot);
                        break;
                    case EResult.InvalidPassword:
                        if (!string.IsNullOrWhiteSpace(_steamBot.LogOnDetails.LoginKey) && _steamBot.LogOnDetails.ShouldRememberPassword)
                        {
                            _steamBot.LogOnDetails.LoginKey = string.Empty;
                            _steamBot.Account.LoginKey = string.Empty;

                            throw new Exception(Resources.Description_InvalidLoginKey);
                        }
                        break;
                    default:
                        switch (loginResult.Result)
                        {
                            case EResult.AccountLogonDenied:
                            case EResult.AccountLogonDeniedVerifiedEmailRequired:
                                CodeType = Infrastructure.Constants.CodeType.Auth;
                                break;
                            case EResult.AccountLoginDeniedNeedTwoFactor:
                                CodeType = Infrastructure.Constants.CodeType.TwoFactor;
                                break;
                        }
                        var errorMsg = await _steamErrorMessageService.GetErrorMessageAsync(loginResult.Result);
                        throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsTryingToLogin = false;
            }
        }
    }
}
