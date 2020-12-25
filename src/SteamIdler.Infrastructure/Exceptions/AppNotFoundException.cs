using System;

namespace SteamIdler.Infrastructure.Exceptions
{
    public class AppNotFoundException : Exception
    {
        public AppNotFoundException(int appId)
        {
            AppId = appId;
        }

        public AppNotFoundException(int appId, string message) : base(message)
        {
            AppId = appId;
        }

        public AppNotFoundException(int appId, string message, Exception innerException) : base(message, innerException)
        {
            AppId = appId;
        }

        public int AppId { get; }
    }
}
