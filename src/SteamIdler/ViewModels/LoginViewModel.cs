using Prism.Commands;
using SteamIdler.Services;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private readonly BotService _botService;

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

        public ICommand SignInCommand { get; }

        public async void SignIn()
        {
            await _botService.LoginAsync(Username);
        }
    }
}
