using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;

namespace SteamIdler.ViewModels
{
    public class ViewModelLocator
    {
        public LoginViewModel Login => ServiceLocator.Current.GetService<LoginViewModel>();
        public MainViewModel Main => ServiceLocator.Current.GetService<MainViewModel>();
    }
}
