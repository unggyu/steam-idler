using System;

namespace SteamIdler.Services
{
    public class PasswordService
    {
        private static PasswordService _instance;

        public static PasswordService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PasswordService();
                }

                return _instance;
            }
        }

        public Func<string> GetPassword;
    }
}
