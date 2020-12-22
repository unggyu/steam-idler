using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;

namespace SteamIdler.ViewModels
{
    public class ViewModelLocator
    {
        public MySplashViewModel MySplash => ServiceLocator.Current.GetService<MySplashViewModel>();
        public LoginViewModel Login => ServiceLocator.Current.GetService<LoginViewModel>();
    }
}
