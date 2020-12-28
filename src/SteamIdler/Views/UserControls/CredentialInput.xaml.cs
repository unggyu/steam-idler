using SteamIdler.Infrastructure.Constants;
using System.Windows;
using System.Windows.Controls;

namespace SteamIdler.Views.UserControls
{
    /// <summary>
    /// CredentialInput.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CredentialInput : UserControl
    {
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register(nameof(Username), typeof(string), typeof(CredentialInput), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(nameof(Password), typeof(string), typeof(CredentialInput), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(nameof(Code), typeof(string), typeof(CredentialInput), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty CodeTextBoxVisibilityProperty = DependencyProperty.Register(nameof(CodeTextBoxVisibility), typeof(Visibility), typeof(CredentialInput), new UIPropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty CodeTypeProperty = DependencyProperty.Register(nameof(CodeType), typeof(CodeType), typeof(CredentialInput), new UIPropertyMetadata(default(CodeType)));

        public CredentialInput()
        {
            InitializeComponent();
        }

        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public Visibility CodeTextBoxVisibility
        {
            get => (Visibility)GetValue(CodeTextBoxVisibilityProperty);
            set => SetValue(CodeTextBoxVisibilityProperty, value);
        }

        public CodeType CodeType
        {
            get => (CodeType)GetValue(CodeTypeProperty);
            set => SetValue(CodeTypeProperty, value);
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }
    }
}
