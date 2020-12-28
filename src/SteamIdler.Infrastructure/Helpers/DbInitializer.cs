using SteamIdler.Infrastructure.Contexts;
using SteamIdler.Infrastructure.Models;
using SteamIdler.Infrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Helpers
{
    public class DbInitializer
    {
        private static DbInitializer _instance;

        public static DbInitializer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DbInitializer();
                }

                return _instance;
            }
        }

        private readonly Repository<App, int> _appRepository;
        private readonly RemoteAppRepository _remoteAppRepository;

        public DbInitializer()
        {
            _appRepository = new Repository<App, int>();
            _remoteAppRepository = RemoteAppRepository.Instance;
        }

        public async Task EnsureInitializeAsync(bool downloadAllApps = false, CancellationToken cancellationToken = default)
        {
            var context = IdlerContext.Instance;
            await context.Database.EnsureCreatedAsync(cancellationToken);

            if (downloadAllApps)
            {
                var localAppsCount = await _appRepository.CountAsync(cancellationToken);
                if (localAppsCount <= 0)
                {
                    var appsEnumerator = _remoteAppRepository.GetAllAppsAsync().GetAsyncEnumerator(cancellationToken);
                    while (await appsEnumerator.MoveNextAsync())
                    {
                        var app = appsEnumerator.Current;
                        await _appRepository.AddAsync(app, cancellationToken);
                    }
                }
            }
        }
    }
}
