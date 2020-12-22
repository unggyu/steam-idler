using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SteamIdler.Views.UserControls
{
    /// <summary>
    /// CredentialInput.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CredentialInput : UserControl
    {
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register(nameof(Username), typeof(string), typeof(CredentialInput));
        public static readonly DependencyProperty SignInCommandProperty = DependencyProperty.Register(nameof(SignInCommand), typeof(ICommand), typeof(CredentialInput));
        public static readonly DependencyProperty SignInButtonIsEnabledProperty = DependencyProperty.Register(nameof(SignInButtonIsEnabled), typeof(bool), typeof(CredentialInput));

        public CredentialInput()
        {
            InitializeComponent();
        }

        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public ICommand SignInCommand
        {
            get => (ICommand)GetValue(SignInCommandProperty);
            set => SetValue(SignInCommandProperty, value);
        }

        public bool SignInButtonIsEnabled
        {
            get => (bool)GetValue(SignInButtonIsEnabledProperty);
            set => SetValue(SignInButtonIsEnabledProperty, value);
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }
    }
}
