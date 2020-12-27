using SteamIdler.Infrastructure.Constants;
using SteamIdler.Infrastructure.Exceptions;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Services;
using SteamKit2;
using SteamKit2.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure
{
    public class SteamBot : Bindable
    {
        private readonly SteamClient _steamClient;
        private readonly CallbackManager _callbackManager;
        private readonly SteamUser _steamUser;
        private readonly Repository<Account, int> _accountRepository;

        private Account _account;
        private bool _isRunning;
        private EResult? _loggedOnResult;
        private CancellationTokenSource _tokenSource;

        public SteamBot(Account account) : this()
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public SteamBot()
        {
            _steamClient = new SteamClient();
            _callbackManager = new CallbackManager(_steamClient);
            _steamUser = _steamClient.GetHandler<SteamUser>();
            _accountRepository = new Repository<Account, int>();

            LogOnDetails = new SteamUser.LogOnDetails();

            _callbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnectedEventHandler);
            _callbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnectedEventHandler);
            _callbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOnEventHandler);
            _callbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOffEventHandler);
            _callbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuthEventHandler);
            _callbackManager.Subscribe<SteamUser.LoginKeyCallback>(OnLoginKeyEventHandler);
        }

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;

                if (_account != null)
                {
                    LogOnDetails.Username = _account.Username;
                    LogOnDetails.Password = _account.Password;
                }
            }
        }

        public bool IsRunning
        {
            get => _isRunning;
        }

        public bool IsRunningApp
        {
            get
            {
                // TODO: 기능 개발
                return false;
            }
        }

        public EResult? LoggedOnResult
        {
            get => _loggedOnResult;
            set
            {
                _loggedOnResult = value;
                OnPropertyChanged();
            }
        }

        public SteamUser.LogOnDetails LogOnDetails { get; }
        public bool IsConnected => _steamClient.IsConnected;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<SteamClient.ConnectedCallback> Connected;
        public event EventHandler<SteamClient.DisconnectedCallback> Disconnected;
        public event EventHandler<SteamUser.LoggedOnCallback> LoggedOn;
        public event EventHandler<SteamUser.LoggedOnCallback> LoginSuccessful;
        public event EventHandler<SteamUser.LoggedOnCallback> LoginFailed;
        public event EventHandler<SteamUser.LoggedOffCallback> LoggedOff;
        public event EventHandler<SteamUser.UpdateMachineAuthCallback> UpdateMachineAuth;
        public event EventHandler<SteamUser.LoginKeyCallback> ReceivedLoginKey;

        public async void ConnectAndWaitCallbacks()
        {
            _isRunning = true;

            if (!IsConnected)
            {
                _steamClient.Connect();

                try
                {
                    _tokenSource = new CancellationTokenSource();

                    await WaitCallbacksAsync(_tokenSource.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    _tokenSource.Dispose();
                }
            }
        }

        public void Disconnect()
        {
            _tokenSource.Cancel();
        }

        public void Login()
        {
            Debug.WriteLine("[Bot.cs] Login");

            if (_account != null)
            {
                if (File.Exists(_account.SentryFilePath))
                {
                    var sentryFile = File.ReadAllBytes(_account.SentryFilePath);
                    LogOnDetails.SentryFileHash = CryptoHelper.SHAHash(sentryFile);
                }
            }

            _steamUser.LogOn(LogOnDetails);
        }

        public void Logout()
        {
            Debug.WriteLine("[Bot.cs] Logout");

            _isRunning = false;
        }

        public void PlayApps(IEnumerable<App> apps = null)
        {
            if (apps == null)
            {
                if (_account == null)
                {
                    throw new ArgumentNullException(nameof(apps));
                }

                apps = _account.AccountApps.Select(aa => aa.App);
            }

            foreach (var app in apps)
            {
                PlayApp(app);
            }
        }

        public void PlayApp(App app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var gamesPlayed = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);
            gamesPlayed.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = (ulong)app.Id
            });
            _steamClient.Send(gamesPlayed);
        }

        private async Task WaitCallbacksAsync(CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                while (IsRunning)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    _callbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                }
            });
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnConnectedEventHandler(SteamClient.ConnectedCallback callback)
        {
            Debug.WriteLine("[Bot.cs] Connected Event Invoke");
            Connected?.Invoke(this, callback);
            OnPropertyChanged(nameof(IsConnected));
        }

        private void OnDisconnectedEventHandler(SteamClient.DisconnectedCallback callback)
        {
            _isRunning = false;

            Debug.WriteLine("[Bot.cs] Disconnected Event Invoke");
            Disconnected?.Invoke(this, callback);
            OnPropertyChanged(nameof(IsConnected));
        }

        private void OnLoggedOnEventHandler(SteamUser.LoggedOnCallback callback)
        {
            Debug.WriteLine("[Bot.cs] LoggedOn Event Invoke");
            LoggedOn?.Invoke(this, callback);

            LoggedOnResult = callback.Result;

            if (callback.Result == EResult.OK)
            {
                LoginSuccessful?.Invoke(this, callback);
            }
            else
            {
                LoginFailed?.Invoke(this, callback);
            }
        }

        private void OnLoggedOffEventHandler(SteamUser.LoggedOffCallback callback)
        {
            Debug.WriteLine("[Bot.cs] LoggedOff Event Invoke");
            LoggedOff?.Invoke(this, callback);
            LoggedOnResult = null;
        }

        private async void OnUpdateMachineAuthEventHandler(SteamUser.UpdateMachineAuthCallback callback)
        {
            int fileSize;
            byte[] sentryHash;
            string sentryFilePath;
            bool isCreatedSentryFile = false;

            if (_account != null)
            {
                sentryFilePath = _account.SentryFilePath;
            }
            else
            {
                isCreatedSentryFile = true;
                var sentryFolderPath = Path.Combine(Directory.GetCurrentDirectory(), LocalFolderNames.Sentries);
                sentryFilePath = Path.Combine(sentryFolderPath, $"{Guid.NewGuid()}.bin");
                if (!Directory.Exists(sentryFolderPath))
                {
                    Directory.CreateDirectory(sentryFolderPath);
                }
            }

            using var fileStream = File.Open(sentryFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.Seek(callback.Offset, SeekOrigin.Begin);
            fileStream.Write(callback.Data, 0, callback.BytesToWrite);

            fileSize = (int)fileStream.Length;

            using var sha = SHA1.Create();
            sentryHash = sha.ComputeHash(fileStream);

            _steamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash
            });

            if (isCreatedSentryFile && _account != null)
            {
                _account.SentryFilePath = sentryFilePath;
                await _accountRepository.EditAsync(_account);
            }
        }

        private async void OnLoginKeyEventHandler(SteamUser.LoginKeyCallback callback)
        {
            LogOnDetails.LoginKey = callback.LoginKey;

            var account = Account ?? await _accountRepository.GetFirstItemAsync(a => a.Username.Equals(LogOnDetails.Username));
            if (account != null)
            {
                account.LoginKey = callback.LoginKey;
                await _accountRepository.EditAsync(account);
            }
            else
            {
                throw new AccountNotFoundException(LogOnDetails.Username);
            }
        }
    }
}
