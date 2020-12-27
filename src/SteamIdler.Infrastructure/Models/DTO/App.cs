using System.Collections.Generic;
using System.Linq;

namespace SteamIdler.Infrastructure.Models
{
    public class App : EntityBase<int>
    {
        private string _name;
        private ICollection<AccountApp> _accountApps;

        public string Name
        {
            get => _name;
            set => SetValue(ref _name, value);
        }

        public virtual ICollection<AccountApp> AccountApps
        {
            get => _accountApps;
            set => SetValue(ref _accountApps, value);
        }

        public override object Clone()
        {
            return new App
            {
                Id = Id,
                Name = Name,
                AccountApps = AccountApps.Select(a => (AccountApp)a.Clone()).ToList()
            };
        }
    }
}
