using SteamIdler.Infrastructure.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Helpers
{
    public class DbInitializer
    {
        public static async Task EnsureInitializeAsync(CancellationToken cancellationToken = default)
        {
            var context = IdlerContext.Instance;
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
