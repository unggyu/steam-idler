using Prism.Commands;
using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Constants;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using SteamIdler.Services;
using SteamKit2;
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

        private ObservableCollection<SteamAccount> _accounts;
        private ObservableCollection<Infrastructure.Models.App> _apps;
        private SteamAccount _selectedAccount;
        private Infrastructure.Models.App _selectedApp;
        private string _appId;

        public MainViewModel()
        {
            _accountRepository = new Repository<Account, int>();
            _appRepository = new Repository<Infrastructure.Models.App, int>();
            _accountAppRepository = new Repository<AccountApp, int>();
            _remoteAppRepository = RemoteAppRepository.Instance;
            _accountService = AccountService.Instance;
            _idlingService = IdlingService.Instance;

            Accounts = new ObservableCollection<SteamAccount>();
            Apps = new ObservableCollection<Infrastructure.Models.App>();

            AddAccountCommand = new DelegateCommand(AddAccount);
            RemoveAccountCommand = new DelegateCommand<object>(DeleteAccount);
            SignInCommand = new DelegateCommand<object>(SignIn);
            AddAppCommand = new DelegateCommand(AddApp);
            RemoveAppCommand = new DelegateCommand<object>(DeleteApp);

            Initialize();
        }

        public ObservableCollection<SteamAccount> Accounts
        {
            get => _accounts;
            set => SetValue(ref _accounts, value);
        }

        public ObservableCollection<Infrastructure.Models.App> Apps
        {
            get => _apps;
            set => SetValue(ref _apps, value);
        }

        public SteamAccount SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                SetValue(ref _selectedAccount, value);
                LoadApps();
            }
        }

        public Infrastructure.Models.App SelectedApp
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
        public ICommand RemoveAccountCommand { get; }
        public ICommand SignInCommand { get; }
        public ICommand AddAppCommand { get; }
        public ICommand RemoveAppCommand { get; }

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
                var steamBot = new SteamBot();
                steamBot.LogOnDetails.Username = account.Username;
                steamBot.LogOnDetails.Password = account.Password;
                var steamAccount = new SteamAccount
                {
                    Account = account,
                    SteamBot = steamBot
                };

                Accounts.Add(steamAccount);
            }

            if (accounts.Count() > 0)
            {
                SelectedAccount = Accounts.FirstOrDefault();
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

        private async void DeleteAccount(object obj)
        {
            if (obj == null || !(obj is SteamAccount account))
            {
                return;
            }

            try
            {
                // TODO: 해당 계정의 앱들 중 아이들링을 하고 있는 앱이 있다면 아이들링을 중단시켜줘야함

                await _accountService.RemoveAccountAsync(account);

                LoadAccounts();
                LoadApps();
            }
            catch (Exception ex)
            {
            }
        }

        private async void SignIn(object obj)
        {
            if (obj == null || !(obj is SteamAccount account))
            {
                return;
            }

            var result = await account.SteamBot.LoginAsync(
                username: account.Account.Username,
                password: account.Account.Password,
                code: account.Code,
                codeType: account.CodeType,
                getPasswordByProvider: false);

            switch (result.Result)
            {
                case EResult.OK:
                    account.IsCodeRequired = false;
                    account.Code = null;
                    account.CodeType = null;
                    break;
                case EResult.AccountLogonDenied:
                case EResult.AccountLogonDeniedVerifiedEmailRequired:
                    await account.SteamBot.AwaitDisconnectAsync();
                    await account.SteamBot.ConnectAsync();
                    account.IsCodeRequired = true;
                    account.CodeType = CodeType.Auth;
                        break;
                case EResult.AccountLoginDeniedNeedTwoFactor:
                    await account.SteamBot.AwaitDisconnectAsync();
                    await account.SteamBot.ConnectAsync();
                    account.IsCodeRequired = true;
                    account.CodeType = CodeType.TwoFactor;
                    break;
                default:
                    await account.SteamBot.AwaitDisconnectAsync();
                    await account.SteamBot.ConnectAsync();
                    break;
            }
        }

        private async void LoadApps(int? appIdToChoose = null)
        {
            if (SelectedAccount == null)
            {
                return;
            }

            Apps.Clear();

            var accountApps = await _accountAppRepository.GetItemsAsync(aa => aa.AccountId == SelectedAccount.Account.Id);
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

            if (appIdToChoose.HasValue)
            {
                SelectedApp = Apps.FirstOrDefault(a => a.Id == appIdToChoose.Value);
            }
            else if (Apps.Count() > 0)
            {
                SelectedApp = Apps.FirstOrDefault();
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
                var accountAppExists = await _accountAppRepository.IsExistsAsync(aa => aa.AccountId == SelectedAccount.Account.Id && aa.AppId == app.Id);
                if (!accountAppExists)
                {
                    await _accountAppRepository.AddAsync(new AccountApp
                    {
                        AccountId = SelectedAccount.Account.Id,
                        AppId = app.Id
                    });

                    LoadApps(appId);

                    AppId = string.Empty;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async void DeleteApp(object app)
        {
            if (app == null || !(app is Infrastructure.Models.App castedApp))
            {
                return;
            }

            try
            {
                // TODO: 만약 해당 앱이 실행 중이라면 종료시켜야함

                var accountApp = await _accountAppRepository.GetFirstItemAsync(aa => aa.AccountId == SelectedAccount.Account.Id && aa.AppId == castedApp.Id);
                if (accountApp == null)
                {
                    return;
                }

                await _accountAppRepository.DeleteAsync(accountApp);

                LoadApps();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
