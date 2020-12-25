using Flurl.Http;
using SteamIdler.Infrastructure.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SteamIdler.Infrastructure.Services
{
    public class RemoteAppRepository
    {
        private static RemoteAppRepository _instance;

        public static RemoteAppRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RemoteAppRepository();
                }

                return _instance;
            }
        }

        private const string GetAppListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v2/";

        public async IAsyncEnumerable<App> GetAllAppsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var result = await GetAppListUrl.GetJsonAsync(cancellationToken);
            foreach (var app in result.applist.apps)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return new App
                {
                    Id = (int)app.appid,
                    Name = app.name
                };
            }
        }
    }
}
