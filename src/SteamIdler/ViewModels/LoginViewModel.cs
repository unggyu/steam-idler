using Prism.Commands;
using Prism.Events;
using SteamIdler.Events;
using SteamIdler.Services;
using SteamKit2;
using System;
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
        private bool _isRequiredAuthCode;
        private bool _isRequiredTwoFactorCode;
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

        public bool IsRequiredAuthCode
        {
            get => _isRequiredAuthCode;
            set
            {
                SetValue(ref _isRequiredAuthCode, value);
                RaisePropertyChanged(nameof(IsCodeRequired));
            }
        }

        public bool IsRequiredTwoFactorCode
        {
            get => _isRequiredAuthCode;
            set
            {
                SetValue(ref _isRequiredTwoFactorCode, value);
                RaisePropertyChanged(nameof(IsCodeRequired));
            }
        }

        private bool IsCodeRequired
        {
            get => IsRequiredAuthCode || IsRequiredTwoFactorCode;
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
                var result = await _botService.LoginAsync(Username);
                switch (result.Result)
                {
                    case EResult.OK:
                        _eventAggregator.GetEvent<LoginSuccessfulEvent>().Publish();
                        break;
                    case EResult.AccountLogonDenied:
                    case EResult.AccountLogonDeniedVerifiedEmailRequired:
                        IsRequiredAuthCode = true;
                        break;
                    case EResult.AccountLoginDeniedNeedTwoFactor:
                        IsRequiredTwoFactorCode = true;
                        break;
                    default:
                        ErrorText = result.Result.ToString();
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
