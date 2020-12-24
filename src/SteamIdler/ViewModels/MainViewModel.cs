using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using System.Collections.ObjectModel;

namespace SteamIdler.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Repository<Account, int> _accountRepository;
        private ObservableCollection<Account> _accounts;

        public MainViewModel()
        {
            Accounts = new ObservableCollection<Account>();

            Initialize();
        }

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetValue(ref _accounts, value);
        }

        public async void Initialize()
        {
            var accounts = await _accountRepository.GetAllItemsAsync();

            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }
    }
}
