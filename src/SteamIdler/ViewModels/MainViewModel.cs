using Prism.Commands;
using SteamIdler.Infrastructure;
using SteamIdler.Infrastructure.Constants;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Repositories;
using SteamIdler.Models;
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
        private readonly AccountRepository _accountRepository;
        private readonly Repository<Infrastructure.Models.App, int> _appRepository;
        private readonly Repository<AccountApp, int> _accountAppRepository;
        private readonly RemoteAppRepository _remoteAppRepository;
        private readonly AccountService _accountService;

        private ObservableCollection<SteamBotForVisual> _bots;
        private ObservableCollection<Infrastructure.Models.App> _apps;
        private SteamBotForVisual _selectedBot;
        private Infrastructure.Models.App _selectedApp;
        private string _appId;

        public MainViewModel()
        {
            _accountRepository = new AccountRepository();
            _appRepository = new Repository<Infrastructure.Models.App, int>();
            _accountAppRepository = new Repository<AccountApp, int>();
            _remoteAppRepository = RemoteAppRepository.Instance;
            _accountService = AccountService.Instance;

            Bots = new ObservableCollection<SteamBotForVisual>();
            Apps = new ObservableCollection<Infrastructure.Models.App>();

            AddAccountCommand = new DelegateCommand(AddAccount);
            RemoveAccountCommand = new DelegateCommand<object>(DeleteAccount);
            SignInCommand = new DelegateCommand<object>(SignIn);
            SignInAllCommand = new DelegateCommand(SignInAll);
            SignOutCommand = new DelegateCommand<object>(SignOut);
            SaveLogOnDetailsCommand = new DelegateCommand<object>(SaveLogOnDetails);
            AddAppCommand = new DelegateCommand(AddApp);
            RemoveAppCommand = new DelegateCommand<object>(DeleteApp);

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

        public ICommand AddAccountCommand { get; }
        public ICommand RemoveAccountCommand { get; }
        public ICommand SignInCommand { get; }
        public ICommand SignInAllCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand SaveLogOnDetailsCommand { get; }
        public ICommand AddAppCommand { get; }
        public ICommand RemoveAppCommand { get; }

        private void Initialize()
        {
            LoadBots();
        }

        private async void LoadBots()
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
                var SteamBotForVisual = new SteamBotForVisual
                {
                    SteamBot = steamBot
                };
                Bots.Add(SteamBotForVisual);
            }

            if (accounts.Count() > 0)
            {
                SelectedBot = Bots.FirstOrDefault();
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

        private async void DeleteAccount(object obj)
        {
            if (obj == null || !(obj is SteamBotForVisual bot))
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
                        bot.IsCodeRequired = false;
                        bot.Code = null;
                        bot.CodeType = null;
                        break;
                    case EResult.AccountLogonDenied:
                    case EResult.AccountLogonDeniedVerifiedEmailRequired:
                        await bot.SteamBot.AwaitDisconnectAsync();
                        await bot.SteamBot.ConnectAsync();
                        bot.IsCodeRequired = true;
                        bot.CodeType = CodeType.Auth;
                        break;
                    case EResult.AccountLoginDeniedNeedTwoFactor:
                        await bot.SteamBot.AwaitDisconnectAsync();
                        await bot.SteamBot.ConnectAsync();
                        bot.IsCodeRequired = true;
                        bot.CodeType = CodeType.TwoFactor;
                        break;
                    default:
                        await bot.SteamBot.AwaitDisconnectAsync();
                        await bot.SteamBot.ConnectAsync();
                        throw new Exception($"{result.Result}");
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

        private async void AddApp()
        {
            if (SelectedBot == null || string.IsNullOrWhiteSpace(AppId))
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
            }
        }
    }
}
