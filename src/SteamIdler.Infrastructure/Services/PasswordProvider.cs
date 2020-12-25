using System;

namespace SteamIdler.Infrastructure.Services
{
    public class PasswordProvider
    {
        private static PasswordProvider _instance;

        public static PasswordProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PasswordProvider();
                }

                return _instance;
            }
        }

        public Func<string> GetPassword;
    }
}
