using SteamIdler.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SteamIdler.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<AccountViewModel>(this, "LoginSuccessful", NavigateToAccountDetailsPage);
        }

        private void NavigateToAccountDetailsPage(AccountViewModel accountViewModel)
        {
            Navigation.PushAsync(new AccountDetailsPage());
        }
    }
}