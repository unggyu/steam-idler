using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Routing;
using SteamIdler.Infrastructure.Exceptions;
using SteamIdler.Infrastructure.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Repositories
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
        private const string GetAppDetailsUrl = "https://store.steampowered.com/api/appdetails";

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

        public async Task<App> GetAppAsync(int appId, CancellationToken cancellationToken = default)
        {
            var result = await GetAppDetailsUrl
                .SetQueryParam("appids", appId)
                .GetJsonAsync(cancellationToken);

            var dictionary = new RouteValueDictionary(result);
            var realResult = (dynamic)dictionary[appId.ToString()];
            if (realResult != null)
            {
                if (realResult.success == false)
                {
                    throw new AppNotFoundException(appId);
                }

                var data = realResult.data;

                return new App
                {
                    Id = (int)data.steam_appid,
                    Name = data.name
                };
            }
            else
            {
                throw new AppNotFoundException(appId);
            }
        }
    }
}
