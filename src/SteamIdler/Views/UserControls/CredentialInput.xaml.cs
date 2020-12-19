using System.Windows;
using System.Windows.Controls;

namespace SteamIdler.Views.UserControls
{
    /// <summary>
    /// CredentialInput.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CredentialInput : UserControl
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(nameof(Id), typeof(string), typeof(CredentialInput));

        public CredentialInput()
        {
            InitializeComponent();
        }

        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        public string GetPassword()
        {
            return PasswordBox.Password;
        }
    }
}
