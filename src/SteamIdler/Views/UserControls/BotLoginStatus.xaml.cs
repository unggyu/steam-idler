using SteamIdler.Infrastructure;
using System.Windows;
using System.Windows.Controls;

namespace SteamIdler.Views.UserControls
{
    /// <summary>
    /// BotLoginStatus.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BotLoginStatus : UserControl
    {
        public static readonly DependencyProperty BotProperty = DependencyProperty.Register(nameof(Bot), typeof(SteamBot), typeof(BotLoginStatus), new UIPropertyMetadata(null));


        public BotLoginStatus()
        {
            InitializeComponent();
        }

        public SteamBot Bot
        {
            get => (SteamBot)GetValue(BotProperty);
            set => SetValue(BotProperty, value);
        }
    }
}
