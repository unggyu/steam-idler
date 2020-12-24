using SteamKit2;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SteamBot
{
    public class Bot : INotifyPropertyChanged
    {
        private readonly SteamClient _steamClient;
        private readonly CallbackManager _callbackManager;
        private readonly SteamUser _steamUser;
        private bool _isRunning;
        private EResult? _loggedOnResult;
        private CancellationTokenSource _tokenSource;

        public Bot(string username, string password) : this()
        {
            LogOnDetails.Username = username;
            LogOnDetails.Password = password;
        }

        public Bot(string username, string password, string loginKey) : this(username, password)
        {
            LogOnDetails.LoginKey = loginKey;
        }

        public Bot()
        {
            _steamClient = new SteamClient();
            _callbackManager = new CallbackManager(_steamClient);

            _steamUser = _steamClient.GetHandler<SteamUser>();

            _callbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnectedEventHandler);
            _callbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnectedEventHandler);
            _callbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOnEventHandler);
            _callbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOffEventHandler);
            _callbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuthEventHandler);
            _callbackManager.Subscribe<SteamUser.LoginKeyCallback>(OnLoginKeyEventHandler);
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

        public SteamUser.LogOnDetails LogOnDetails { get; set; } = new SteamUser.LogOnDetails();
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

            if (File.Exists("sentry.bin"))
            {
                var sentryFile = File.ReadAllBytes("sentry.bin");

                LogOnDetails.SentryFileHash = CryptoHelper.SHAHash(sentryFile);
            }

            _steamUser.LogOn(LogOnDetails);
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

        private void OnUpdateMachineAuthEventHandler(SteamUser.UpdateMachineAuthCallback callback)
        {
            int fileSize;
            byte[] sentryHash;

            using (var fs = File.Open("sentry.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);

                fileSize = (int)fs.Length;

                using (var sha = SHA1.Create())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }

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
        }

        private void OnLoginKeyEventHandler(SteamUser.LoginKeyCallback callback)
        {
            LogOnDetails.LoginKey = callback.LoginKey;
        }
    }
}
