using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using SteamIdler.ViewModels;
using SteamIdler.Views;
using System;
using System.Windows;

namespace SteamIdler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            ServiceProvider = ConfigureServices(new ServiceCollection());
            ServiceLocator.SetLocatorProvider(() => new MSDIServiceLocator(ServiceProvider));
        }

        private IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<MySplashViewModel>()
                .AddScoped<LoginViewModel>();

            services
                .AddScoped<IEventAggregator, EventAggregator>();

            return services.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var splashScreen = new MySplashScreen();
            var result = splashScreen.ShowDialog();
            if (result == null || !result.Value)
            {
                Shutdown();
            }

            try
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
