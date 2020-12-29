using SteamIdler.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Repositories
{
    public interface IReadonlyRepository<TEntity, TKey> where TEntity : EntityBase<TKey> where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllItemsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetItemsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> GetFirstItemAsync(CancellationToken cancellationToken = default);
        Task<TEntity> GetFirstItemAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity> GetLastItemAsync(CancellationToken cancellationToken = default);
        Task<TEntity> GetLastItemAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
