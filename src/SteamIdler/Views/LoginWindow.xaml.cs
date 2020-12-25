using CommonServiceLocator;
using Prism.Events;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SteamIdler.Events;
using SteamIdler.Infrastructure.Services;
using SteamIdler.Infrastructure;

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
                SteamBot = bot;
                DialogResult = true;
            });
        }

        public SteamBot SteamBot { get; set; }

        public void Initialize()
        {
            var passwordService = PasswordProvider.Instance;
            passwordService.GetPassword = CredentialInput.GetPassword;
        }
    }
}
