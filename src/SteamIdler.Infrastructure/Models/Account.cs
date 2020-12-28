using System.Collections.Generic;
using System.Linq;

namespace SteamIdler.Infrastructure.Models
{
    public class Account : EntityBase<int>
    {
        private string _username;
        private string _password;
        private bool _rememberPassword;
        private bool _automaticLogin;
        private string _loginKey;
        private string _sentryFilePath;
        private ICollection<AccountApp> _accountApps;

        public string Username
        {
            get => _username;
            set => SetValue(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        public bool RememberPassword
        {
            get => _rememberPassword;
            set => SetValue(ref _rememberPassword, value);
        }

        public bool AutomaticLogin
        {
            get => _automaticLogin;
            set => SetValue(ref _automaticLogin, value);
        }

        public string LoginKey
        {
            get => _loginKey;
            set => SetValue(ref _loginKey, value);
        }

        public string SentryFilePath
        {
            get => _sentryFilePath;
            set => SetValue(ref _sentryFilePath, value);
        }

        public virtual ICollection<AccountApp> AccountApps
        {
            get => _accountApps;
            set => SetValue(ref _accountApps, value);
        }

        public override object Clone()
        {
            return new Account
            {
                Id = Id,
                Username = Username,
                Password = Password,
                RememberPassword = RememberPassword,
                AutomaticLogin = AutomaticLogin,
                LoginKey = LoginKey,
                SentryFilePath = SentryFilePath,
                AccountApps = AccountApps.Select(a => (AccountApp)a.Clone()).ToList()
            };
        }
    }
}
