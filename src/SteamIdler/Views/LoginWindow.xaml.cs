using SteamIdler.Services;
using System.Windows;

namespace SteamIdler.Views
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            var passwordService = PasswordService.Instance;
            passwordService.GetPassword = CredentialInput.GetPassword;
        }
    }
}
