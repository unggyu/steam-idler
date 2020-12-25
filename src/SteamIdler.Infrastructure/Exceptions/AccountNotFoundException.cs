using System;

namespace SteamIdler.Infrastructure.Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(string username)
        {
            Username = username;
        }

        public AccountNotFoundException(string username, string message) : base(message)
        {
            Username = username;
        }

        public AccountNotFoundException(string username, string message, Exception innerException) : base(message, innerException)
        {
            Username = username;
        }

        public string Username { get; }
    }
}
