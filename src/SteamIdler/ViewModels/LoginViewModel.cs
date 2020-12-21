using Prism.Commands;
using SteamIdler.Services;
using System;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly BotService _botService;
        private string _username;
        private string _errorText;

        public LoginViewModel()
        {
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

        public ICommand SignInCommand { get; }

        public async void SignIn()
        {
            try
            {
                await _botService.LoginAsync(Username);
            }
            catch (Exception ex)
            {
                ErrorText = ex.Message;
            }
        }
    }
}
