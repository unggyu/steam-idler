using CommonServiceLocator;
using Prism.Events;
using SteamIdler.Services;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SteamIdler.Events;

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
            _eventAggregator.GetEvent<LoginSuccessfulEvent>().Subscribe(() => DialogResult = true);
        }

        public void Initialize()
        {
            var passwordService = PasswordService.Instance;
            passwordService.GetPassword = CredentialInput.GetPassword;
        }
    }
}
