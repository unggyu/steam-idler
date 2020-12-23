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

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var splashScreen = new MySplashScreen();
            var splashScreenResult = splashScreen.ShowDialog();
            if (splashScreenResult == null || !splashScreenResult.Value)
            {
                Shutdown();
            }

            var loginWindow = new LoginWindow();
            var loginResult = loginWindow.ShowDialog();
            if (loginResult == null || !loginResult.Value)
            {
                Shutdown();
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
