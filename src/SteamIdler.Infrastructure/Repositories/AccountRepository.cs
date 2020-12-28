using SteamIdler.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Repositories
{
    public class AccountRepository : Repository<Account, int>
    {
        public override Task AddAsync(Account entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!entity.RememberPassword)
            {
                entity.Password = null;
            }

            return base.AddAsync(entity, cancellationToken);
        }

        public override Task EditAsync(Account entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (!entity.RememberPassword)
            {
                entity.Password = null;
            }

            return base.EditAsync(entity, cancellationToken);
        }
    }
}
