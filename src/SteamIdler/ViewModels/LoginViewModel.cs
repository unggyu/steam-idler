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

        public ICommand SignInCommand { get; }

        public async void SignIn()
        {
            try
            {
                var result = await _botService.LoginAsync(Username);
                if (result.Result == EResult.OK)
                {
                    _eventAggregator.GetEvent<LoginSuccessfulEvent>().Publish();
                }
                else
                {
                    ErrorText = result.Result.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
    }
}
