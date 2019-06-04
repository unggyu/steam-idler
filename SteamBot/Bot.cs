using SteamKit2;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SteamBot
{
    public class Bot : INotifyPropertyChanged
    {
        #region Fields
        private readonly SteamClient _steamClient;
        private readonly CallbackManager _callbackManager;
        private readonly SteamUser _steamUser;
        private bool _isRunning;
        private bool _isLoggedOn;
        #endregion

        #region Constructors
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

            #region Subscribe Callbacks
            _callbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnectedEventHandler);
            _callbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnectedEventHandler);

            _callbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOnEventHandler);
            _callbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOffEventHandler);

            _callbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnUpdateMachineAuthEventHandler);

            _callbackManager.Subscribe<SteamUser.LoginKeyCallback>(OnLoginKeyEventHandler);
            #endregion
        }
        #endregion

        #region Properties
        public SteamUser.LogOnDetails LogOnDetails { get; set; } = new SteamUser.LogOnDetails();
        public bool IsConnected => _steamClient.IsConnected;

        public bool IsLoggedOn
        {
            get => _isLoggedOn;
            set
            {
                _isLoggedOn = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<SteamClient.ConnectedCallback> Connected;
        public event EventHandler<SteamClient.DisconnectedCallback> Disconnected;
        public event EventHandler<SteamUser.LoggedOnCallback> LoggedOn;
        public event EventHandler<SteamUser.LoggedOffCallback> LoggedOff;
        public event EventHandler<SteamUser.UpdateMachineAuthCallback> UpdateMachineAuth;
        public event EventHandler<SteamUser.LoginKeyCallback> ReceivedLoginKey;
        public event EventHandler LoginSuccessful;
        #endregion

        #region Public Methods
        public async Task ConnectAndWAitCallbacksAsync(CancellationToken token = default)
        {
            _isRunning = true;

            if (!IsConnected)
            {
                _steamClient.Connect();

                await WaitCallbacks(token);
            }
        }

        public void Login()
        {
            if (File.Exists("sentry.bin"))
            {
                var sentryFile = File.ReadAllBytes("sentry.bin");

                LogOnDetails.SentryFileHash = CryptoHelper.SHAHash(sentryFile);
            }

            _steamUser.LogOn(LogOnDetails);
        }
        #endregion

        #region Private Methods
        private async Task WaitCallbacks(CancellationToken token = default)
        {
            await Task.Run(() =>
            {
                while (_isRunning)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    _callbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
                }
            });
        }

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Event Handlers
        private void OnConnectedEventHandler(SteamClient.ConnectedCallback callback)
        {
            Connected?.Invoke(this, callback);
            OnPropertyChanged(nameof(IsConnected));
        }

        private void OnDisconnectedEventHandler(SteamClient.DisconnectedCallback callback)
        {
            _isRunning = false;

            Disconnected?.Invoke(this, callback);
            OnPropertyChanged(nameof(IsConnected));
        }

        private void OnLoggedOnEventHandler(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                IsLoggedOn = true;

                LoginSuccessful?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnLoggedOffEventHandler(SteamUser.LoggedOffCallback callback)
        {
            LoggedOff?.Invoke(this, callback);
            IsLoggedOn = false;
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
        #endregion
    }
}
