using System.Windows;
using SteamIdler.Infrastructure.Helpers;
using System;

namespace SteamIdler.Views
{
    /// <summary>
    /// MySplashScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MySplashScreen : Window
    {
        public MySplashScreen()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private async void Initialize()
        {
            TaskContentRun.Text = Properties.Resources.Loading;

            try
            {
                var initializer = DbInitializer.Instance;
                await initializer.EnsureInitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");

                DialogResult = false;

                return;
            }

            DialogResult = true;
        }
    }
}
