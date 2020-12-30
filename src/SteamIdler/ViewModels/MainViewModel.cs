using Prism.Commands;
using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Constants;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Repositories;
using SteamIdler.Models;
using SteamIdler.Properties;
using SteamIdler.Services;
using SteamKit2;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace SteamIdler.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ISteamErrorMessageService _steamErrorMessageService;
        private readonly IRepository<Account, int> _accountRepository;
        private readonly IRepository<Infrastructure.Models.App, int> _appRepository;
        private readonly IAccountService _accountService;
        private readonly IRepository<AccountApp, int> _accountAppRepository;
        private readonly IRemoteAppRepository _remoteAppRepository;

        private ObservableCollection<SteamBotForVisual> _bots;
        private ObservableCollection<Infrastructure.Models.App> _apps;
        private SteamBotForVisual _selectedBot;
        private Infrastructure.Models.App _selectedApp;
        private string _appId;

        private ICommand _addAccountCommand;
        private ICommand _removeAccountCommand;
        private ICommand _signInCommand;
        private ICommand _signInAllCommand;
        private ICommand _signOutCommand;
        private ICommand _saveLogOnDetailsCommand;
        private ICommand _addAppCommand;
        private ICommand _removeAppCommand;

        public MainViewModel(
            IDialogService dialogService,
            ISteamErrorMessageService steamErrorMessageService,
            IRepository<Account, int> accountRepository,
            IRepository<Infrastructure.Models.App, int> appRepository,
            IRepository<AccountApp, int> accountAppRepository,
            IRemoteAppRepository remoteAppRepository,
            IAccountService accountService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _steamErrorMessageService = steamErrorMessageService ?? throw new ArgumentNullException(nameof(steamErrorMessageService));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _appRepository = appRepository ?? throw new ArgumentNullException(nameof(appRepository));
            _accountAppRepository = accountAppRepository ?? throw new ArgumentNullException(nameof(accountAppRepository));
            _remoteAppRepository = remoteAppRepository ?? throw new ArgumentNullException(nameof(remoteAppRepository));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

            Bots = new ObservableCollection<SteamBotForVisual>();
            Apps = new ObservableCollection<Infrastructure.Models.App>();

            Initialize();
        }

        public ObservableCollection<SteamBotForVisual> Bots
        {
            get => _bots;
            set => SetValue(ref _bots, value);
        }

        public bool IsAllBotsLoggedIn
        {
            get => _bots.All(b => b.SteamBot.IsLoggedOn);
        }

        public SteamBotForVisual SelectedBot
        {
            get => _selectedBot;
            set
            {
                SetValue(ref _selectedBot, value);
                LoadApps();
            }
        }

        public ObservableCollection<Infrastructure.Models.App> Apps
        {
            get => _apps;
            set => SetValue(ref _apps, value);
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

        public ICommand AddAccountCommand
        {
            get => _addAccountCommand ??= new DelegateCommand(AddAccount);
        }

        public ICommand RemoveAccountCommand
        {
            get => _removeAccountCommand ??= new DelegateCommand<object>(RemoveAccount);
        }

        public ICommand SignInCommand
        {
            get => _signInCommand ??= new DelegateCommand<object>(SignIn);
        }

        public ICommand SignInAllCommand
        {
            get => _signInAllCommand ??= new DelegateCommand(SignInAll);
        }

        public ICommand SignOutCommand
        {
            get => _signOutCommand ??= new DelegateCommand<object>(SignOut);
        }

        public ICommand SaveLogOnDetailsCommand
        {
            get => _saveLogOnDetailsCommand ??= new DelegateCommand<object>(SaveLogOnDetails);
        }

        public ICommand AddAppCommand
        {
            get => _addAppCommand ??= new DelegateCommand(AddApp);
        }

        public ICommand RemoveAppCommand
        {
            get => _removeAppCommand ??= new DelegateCommand<object>(RemoveApp);
        }

        private void Initialize()
        {
            LoadBots();
        }

        private async void LoadBots()
        {
            try
            {
                Bots.Clear();

                var accounts = await _accountRepository.GetAllItemsAsync();
                foreach (var account in accounts)
                {
                    var steamBot = new SteamBot
                    {
                        Account = account
                    };
                    steamBot.LoggedOn += (obj, callback) => RaisePropertyChanged(nameof(IsAllBotsLoggedIn));
                    steamBot.LoggedOff += (obj, callback) => RaisePropertyChanged(nameof(IsAllBotsLoggedIn));
                    steamBot.Disconnected += (obj, callback) => RaisePropertyChanged(nameof(IsAllBotsLoggedIn));
                    var steamBotForVisual = new SteamBotForVisual
                    {
                        SteamBot = steamBot
                    };
                    Bots.Add(steamBotForVisual);

                    if (account.AutomaticLogin)
                    {
                        SignIn(steamBotForVisual);
                    }
                }

                if (accounts.Count() > 0)
                {
                    SelectedBot = Bots.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel.cs] {ex.Message}");
                _dialogService.ShowErrorMessage(Resources.Error, Resources.Description_LoadAccountsError);
            }
        }

        private async void AddAccount()
        {
            var account = await _accountService.AddAccountAsync();
            if (account == null)
            {
                return;
            }

            LoadBots();
        }

        private async void RemoveAccount(object obj)
        {
            var dialogResult = await _dialogService.ShowDeleteAccountDialogAsync();

            if (obj == null || !(obj is SteamBotForVisual bot) || !dialogResult)
            {
                return;
            }

            try
            {
                // TODO: 해당 계정의 앱들 중 아이들링을 하고 있는 앱이 있다면 아이들링을 중단시켜줘야함

                await _accountRepository.DeleteAsync(bot.SteamBot.Account);

                LoadBots();
                LoadApps();
            }
            catch (Exception ex)
            {
                bot.HasError = true;
                bot.ErrorMessage = ex.Message;
            }
        }

        private async void SignIn(object obj)
        {
            if (obj == null || !(obj is SteamBotForVisual bot))
            {
                return;
            }

            try
            {
                var result = await bot.SteamBot.LoginAsync(
                    username: bot.SteamBot.Account.Username,
                    password: bot.SteamBot.Account.Password,
                    code: bot.Code,
                    codeType: bot.CodeType,
                    getPasswordByProvider: false);

                switch (result.Result)
                {
                    case EResult.OK:
                        bot.HasError = false;
                        bot.ErrorMessage = null;
                        bot.Code = null;
                        bot.CodeType = null;
                        break;
                    case EResult.InvalidPassword:
                        if (!string.IsNullOrWhiteSpace(bot.SteamBot.LogOnDetails.LoginKey) && bot.SteamBot.LogOnDetails.ShouldRememberPassword)
                        {
                            bot.SteamBot.LogOnDetails.LoginKey = string.Empty;
                            bot.SteamBot.Account.LoginKey = string.Empty;

                            throw new Exception(Resources.Description_InvalidLoginKey);
                        }
                        break;
                    default:
                        await bot.SteamBot.AwaitDisconnectAsync();
                        await bot.SteamBot.ConnectAsync();
                        switch (result.Result)
                        {
                            case EResult.AccountLogonDenied:
                            case EResult.AccountLogonDeniedVerifiedEmailRequired:
                            case EResult.InvalidLoginAuthCode:
                                bot.CodeType = CodeType.Auth;
                                break;
                            case EResult.AccountLoginDeniedNeedTwoFactor:
                            case EResult.TwoFactorCodeMismatch:
                                bot.CodeType = CodeType.TwoFactor;
                                break;
                        }
                        var errorMsg = await _steamErrorMessageService.GetErrorMessageAsync(result.Result);
                        throw new Exception(errorMsg);
                }
            }
            catch (Exception ex)
            {
                bot.HasError = true;
                bot.ErrorMessage = ex.Message;
            }
        }

        private void SignInAll()
        {
            foreach (var bot in _bots)
            {
                if (!bot.SteamBot.IsLoggedOn)
                {
                    SignIn(bot);
                }
            }
        }

        private async void SignOut(object obj)
        {
            if (obj == null || !(obj is SteamBotForVisual bot))
            {
                return;
            }

            try
            {
                var result = await bot.SteamBot.LogoutAsync();
            }
            catch (Exception ex)
            {
                bot.HasError = true;
                bot.ErrorMessage = ex.Message;
            }
        }

        private async void SaveLogOnDetails(object obj)
        {
            if (obj == null || !(obj is SteamBot bot))
            {
                return;
            }

            bot.Account.Username = bot.LogOnDetails.Username;
            bot.Account.Password = bot.LogOnDetails.Password;
            bot.Account.AutomaticLogin = bot.LogOnDetails.ShouldRememberPassword;

            await _accountRepository.EditAsync(bot.Account);
        }

        private async void LoadApps(int? appIdToChoose = null)
        {
            try
            {
                if (SelectedBot == null)
                {
                    return;
                }

                Apps.Clear();

                var accountApps = await _accountAppRepository.GetItemsAsync(aa => aa.AccountId == SelectedBot.SteamBot.Account.Id);
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel.cs] {ex.Message}");
                _dialogService.ShowErrorMessage(Resources.Error, Resources.Description_LoadAppsError);
            }
        }

        private async void AddApp()
        {
            try
            {
                if (SelectedBot == null || string.IsNullOrWhiteSpace(AppId))
                {
                    return;
                }

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
                var accountAppExists = await _accountAppRepository.IsExistsAsync(aa => aa.AccountId == SelectedBot.SteamBot.Account.Id && aa.AppId == app.Id);
                if (!accountAppExists)
                {
                    await _accountAppRepository.AddAsync(new AccountApp
                    {
                        AccountId = SelectedBot.SteamBot.Account.Id,
                        AppId = app.Id
                    });

                    LoadApps(appId);

                    AppId = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel.cs] {ex.Message}");
                _dialogService.ShowErrorMessage(Resources.Error, Resources.Description_AddAppError);
            }
        }

        private async void RemoveApp(object app)
        {
            if (app == null || !(app is Infrastructure.Models.App castedApp))
            {
                return;
            }

            try
            {
                // TODO: 만약 해당 앱이 실행 중이라면 종료시켜야함

                var accountApp = await _accountAppRepository.GetFirstItemAsync(aa => aa.AccountId == SelectedBot.SteamBot.Account.Id && aa.AppId == castedApp.Id);
                if (accountApp == null)
                {
                    return;
                }

                await _accountAppRepository.DeleteAsync(accountApp);

                LoadApps();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainViewModel.cs] {ex.Message}");
                _dialogService.ShowErrorMessage(Resources.Error, Resources.Description_DeleteAppError);
            }
        }
    }
}
