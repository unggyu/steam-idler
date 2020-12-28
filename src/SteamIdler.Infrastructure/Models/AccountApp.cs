namespace SteamIdler.Infrastructure.Models
{
    public class AccountApp : EntityBase<int>
    {
        private int _accountId;
        private Account _account;
        private int _appId;
        private App _app;

        public int AccountId
        {
            get => _accountId;
            set => SetValue(ref _accountId, value);
        }

        public virtual Account Account
        {
            get => _account;
            set => SetValue(ref _account, value);
        }

        public int AppId
        {
            get => _appId;
            set => SetValue(ref _appId, value);
        }

        public virtual App App
        {
            get => _app;
            set => SetValue(ref _app, value);
        }

        public override object Clone()
        {
            return new AccountApp
            {
                Id = Id,
                AccountId = AccountId,
                Account = (Account)Account.Clone(),
                AppId = AppId,
                App = (App)App.Clone()
            };
        }
    }
}
