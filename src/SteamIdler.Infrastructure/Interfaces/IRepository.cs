using SteamIdler.Infrastructure.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Interfaces
{
    public interface IRepository<TEntity, TKey> : IReadonlyRepository<TEntity, TKey> where TEntity : EntityBase<TKey> where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task EditAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}
