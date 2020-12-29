using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Repositories;
using SteamIdler.Services;
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
                .AddScoped<LoginViewModel>()
                .AddScoped<MainViewModel>();

            services
                .AddScoped<IDialogService, DialogService>()
                .AddScoped<IEventAggregator, EventAggregator>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IRepository<Account, int>, AccountRepository>()
                .AddScoped<IRepository<Infrastructure.Models.App, int>, Repository<Infrastructure.Models.App, int>>()
                .AddScoped<IRepository<AccountApp, int>, Repository<AccountApp, int>>()
                .AddScoped<IRemoteAppRepository, RemoteAppRepository>();

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

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
