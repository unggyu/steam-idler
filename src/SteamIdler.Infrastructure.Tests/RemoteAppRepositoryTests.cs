using SteamIdler.Infrastructure.Services;
using Xunit;

namespace SteamIdler.Infrastructure.Tests
{
    public class RemoteAppRepositoryTests
    {
        [Fact]
        public async void GetAllAppsAsyncTest()
        {
            var repository = RemoteAppRepository.Instance;
            var appsEnumerator = repository.GetAllAppsAsync().GetAsyncEnumerator();
            while (await appsEnumerator.MoveNextAsync())
            {
                Assert.NotNull(appsEnumerator.Current);
            }
        }

        [Fact]
        public async void GetAppAsyncTest()
        {
            var repository = RemoteAppRepository.Instance;
            var app = await repository.GetAppAsync(220);
            Assert.NotNull(app);
        }
    }
}
