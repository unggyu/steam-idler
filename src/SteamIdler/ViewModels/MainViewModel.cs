using Prism.Commands;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using SteamIdler.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Repository<Account, int> _accountRepository;
        private readonly AccountService _accountService;
        private ObservableCollection<Account> _accounts;
        private ObservableCollection<Infrastructure.Models.App> _apps;
        private Account _selectedAccount;
        private App _selectedApp;
        private string _searchKeyword;

        public MainViewModel()
        {
            _accountRepository = new Repository<Account, int>();
            _accountService = AccountService.Instance;
            Accounts = new ObservableCollection<Account>();
            Apps = new ObservableCollection<Infrastructure.Models.App>();

            AddAccountCommand = new DelegateCommand(AddAccount);

            Initialize();
        }

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetValue(ref _accounts, value);
        }

        public ObservableCollection<Infrastructure.Models.App> Apps
        {
            get => _apps;
            set => SetValue(ref _apps, value);
        }

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                SetValue(ref _selectedAccount, value);
                LoadApps();
            }
        }

        public App SelectedApp
        {
            get => _selectedApp;
            set => SetValue(ref _selectedApp, value);
        }

        public string SearchKeyword
        {
            get => _searchKeyword;
            set => SetValue(ref _searchKeyword, value);
        }

        public ICommand AddAccountCommand { get; }

        private void Initialize()
        {
            LoadAccounts();
        }

        private async void LoadAccounts()
        {
            Accounts.Clear();

            var accounts = await _accountRepository.GetAllItemsAsync();

            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }

        private async void AddAccount()
        {
            var account = await _accountService.AddAccountAsync();
            if (account == null)
            {
                return;
            }

            LoadAccounts();
        }

        private void LoadApps()
        {
            Apps.Clear();

            var apps = SelectedAccount?.AccountApps.Select(aa => aa.App);
            if (apps == null)
            {
                return;
            }

            foreach (var app in apps)
            {
                Apps.Add(app);
            }
        }
    }
}
