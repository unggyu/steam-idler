using System;

namespace SteamIdler.Infrastructure.Exceptions
{
    public class SteamBotNotFoundException : Exception
    {
        public SteamBotNotFoundException(string username)
        {
            Username = username;
        }

        public SteamBotNotFoundException(string username, string message) : base(message)
        {
            Username = username;
        }

        public SteamBotNotFoundException(string username, string message, Exception innerException) : base(message, innerException)
        {
            Username = username;
        }

        public string Username { get; }
    }
}
