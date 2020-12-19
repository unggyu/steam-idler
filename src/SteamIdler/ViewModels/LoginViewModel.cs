using Prism.Commands;
using SteamBot;
using SteamIdler.Services;
using System;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private readonly Bot _bot;

        public LoginViewModel()
        {
            _bot = new Bot();

            SignInCommand = new DelegateCommand(SignIn);
        }

        public string Username
        {
            get => _username;
            set => SetValue(ref _username, value);
        }

        public ICommand SignInCommand { get; }

        public void SignIn()
        {
            _bot.LogOnDetails.Username = Username;

            var passwordService = PasswordService.Instance;
            if (passwordService.GetPassword == null)
            {
                throw new Exception("GetPassword function not found.");
            }
            _bot.LogOnDetails.Password = passwordService.GetPassword();
            _bot.Login();
        }
    }
}
