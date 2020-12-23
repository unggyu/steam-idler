using System.Collections.Generic;
using System.Linq;

namespace SteamIdler.Infrastructure.Models
{
    public class Account : EntityBase<int>
    {
        private string _username;
        private string _password;
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
                AccountApps = AccountApps.Select(a => (AccountApp)a.Clone()).ToList()
            };
        }
    }
}
