using CommonServiceLocator;
using Prism.Events;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SteamIdler.Events;

namespace SteamIdler.Views
{
    /// <summary>
    /// MySplashScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MySplashScreen : Window
    {
        private readonly IEventAggregator _eventAggregator;

        public MySplashScreen()
        {
            InitializeComponent();

            _eventAggregator = ServiceLocator.Current.GetService<IEventAggregator>();
            _eventAggregator.GetEvent<InitializedEvent>().Subscribe(result =>
            {
                DialogResult = true;
            });
        }
    }
}
