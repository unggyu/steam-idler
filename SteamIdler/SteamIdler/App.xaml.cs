using Autofac;
using SteamIdler.Context;
using SteamIdler.Helpers;
using SteamIdler.Views;
using Xamarin.Forms;

namespace SteamIdler
{
    public partial class App : Application
    {
        public static IContainer Container { get; set; }

        public App()
        {
            InitializeComponent();
            InitializeContainer();

            MainPage = new MainPage();
        }

        public static void InitializeContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SteamContextHelper<SteamContext>>();

            builder.Build();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
