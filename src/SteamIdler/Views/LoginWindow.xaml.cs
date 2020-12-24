using CommonServiceLocator;
using Prism.Events;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SteamIdler.Events;
using SteamIdler.Infrastructure.Services;
using SteamBot;

namespace SteamIdler.Views
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IEventAggregator _eventAggregator;

        public LoginWindow()
        {
            InitializeComponent();
            Initialize();

            _eventAggregator = ServiceLocator.Current.GetService<IEventAggregator>();
            _eventAggregator.GetEvent<LoginSuccessfulEvent>().Subscribe(bot =>
            {
                Bot = bot;
                DialogResult = true;
            });
        }

        public Bot Bot { get; set; }

        public void Initialize()
        {
            var passwordService = PasswordService.Instance;
            passwordService.GetPassword = CredentialInput.GetPassword;
        }
    }
}
