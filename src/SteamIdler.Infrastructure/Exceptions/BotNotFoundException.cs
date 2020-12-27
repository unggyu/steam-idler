using System;

namespace SteamIdler.Infrastructure.Exceptions
{
    public class BotNotFoundException : Exception
    {
        public BotNotFoundException(string username)
        {
            Username = username;
        }

        public BotNotFoundException(string username, string message) : base(message)
        {
            Username = username;
        }

        public BotNotFoundException(string username, string message, Exception innerException) : base(message, innerException)
        {
            Username = username;
        }

        public string Username { get; }
    }
}
