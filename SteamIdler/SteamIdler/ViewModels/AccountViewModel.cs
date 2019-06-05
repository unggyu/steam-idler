using GalaSoft.MvvmLight.Command;
using SteamBot;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace SteamIdler.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        #region Constructors
        public AccountViewModel()
        {
            LoginCommand = new RelayCommand(Login);

            SteamBot = new Bot();
            SteamBot.ConnectAndWaitCallbacksAsync();
            SteamBot.LoginSuccessful += SteamBot_LoginSuccessful;
        }
        #endregion

        #region Properties
        public Bot SteamBot { get; set; }
        #endregion

        #region Commands
        public ICommand LoginCommand { get; set; }
        #endregion

        #region Private Methods
        private void Login()
        {
            SteamBot.Login();
        }
        #endregion

        #region Event Handlers
        private void SteamBot_LoginSuccessful(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "LoginSuccessful");
        }
        #endregion
    }
}
