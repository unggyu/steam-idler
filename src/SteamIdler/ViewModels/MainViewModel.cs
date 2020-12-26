using Prism.Commands;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using SteamIdler.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Repository<Account, int> _accountRepository;
        private readonly Repository<Infrastructure.Models.App, int> _appRepository;
        private readonly Repository<AccountApp, int> _accountAppRepository;
        private readonly RemoteAppRepository _remoteAppRepository;
        private readonly AccountService _accountService;
        private readonly IdlingService _idlingService;

        private ObservableCollection<Account> _accounts;
        private ObservableCollection<Infrastructure.Models.App> _apps;
        private Account _selectedAccount;
        private App _selectedApp;
        private string _appId;

        public MainViewModel()
        {
            _accountRepository = new Repository<Account, int>();
            _appRepository = new Repository<Infrastructure.Models.App, int>();
            _accountAppRepository = new Repository<AccountApp, int>();
            _remoteAppRepository = RemoteAppRepository.Instance;
            _accountService = AccountService.Instance;
            _idlingService = IdlingService.Instance;

            Accounts = new ObservableCollection<Account>();
            Apps = new ObservableCollection<Infrastructure.Models.App>();

            AddAccountCommand = new DelegateCommand(AddAccount);
            AddAppCommand = new DelegateCommand(AddApp);

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

        public string AppId
        {
            get => _appId;
            set => SetValue(ref _appId, value);
        }

        public ICommand AddAccountCommand { get; }
        public ICommand AddAppCommand { get; }

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

        private async void LoadApps()
        {
            if (SelectedAccount == null)
            {
                return;
            }

            Apps.Clear();

            var accountApps = await _accountAppRepository.GetItemsAsync(aa => aa.AccountId == SelectedAccount.Id);
            if (accountApps == null)
            {
                return;
            }

            var apps = accountApps.Select(aa => aa.App);
            foreach (var app in apps)
            {
                if (app != null)
                {    
                    Apps.Add(app);
                }
            }
        }

        private async void AddApp()
        {
            if (SelectedAccount == null || string.IsNullOrWhiteSpace(AppId))
            {
                return;
            }

            try
            {
                var appId = int.Parse(AppId);
                var app = await _remoteAppRepository.GetAppAsync(appId);
                if (app == null)
                {
                    throw new Exception($"{nameof(app)} is null.");
                }

                var appExists = await _appRepository.IsExistsAsync(a => a.Id == appId);
                if (!appExists)
                {
                    await _appRepository.AddAsync(app);
                }
                var accountAppExists = await _accountAppRepository.IsExistsAsync(aa => aa.AccountId == SelectedAccount.Id && aa.AppId == app.Id);
                if (!accountAppExists)
                {
                    await _accountAppRepository.AddAsync(new AccountApp
                    {
                        AccountId = SelectedAccount.Id,
                        AppId = app.Id
                    });

                    LoadApps();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
