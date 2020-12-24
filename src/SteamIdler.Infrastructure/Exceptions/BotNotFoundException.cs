using System;

namespace SteamIdler.Infrastructure.Exceptions
{
    public class BotNotFoundException : Exception
    {
        public BotNotFoundException()
        {
        }

        public BotNotFoundException(string message) : base(message)
        {
        }

        public BotNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
