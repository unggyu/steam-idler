using Microsoft.EntityFrameworkCore;
using SteamIdler.Infrastructure.Contexts;
using SteamIdler.Infrastructure.Interfaces;
using SteamIdler.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Infrastructure.Services
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : EntityBase<TKey> where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
    {
        private readonly IdlerContext _context;

        public Repository()
        {
            _context = IdlerContext.Instance;
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            var count = await _context.Set<TEntity>().CountAsync(cancellationToken);
            return count;
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var count = await _context.Set<TEntity>().CountAsync(predicate, cancellationToken);
            return count;
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var trackedEntity = await GetByIdAsync(entity.Id, cancellationToken);
            if (trackedEntity != null)
            {
                _context.Set<TEntity>().Remove(trackedEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual async Task EditAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllItemsAsync(CancellationToken cancellationToken = default)
        {
            var items = await _context.Set<TEntity>().ToListAsync(cancellationToken);
            return items;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var item = await _context.Set<TEntity>().FirstOrDefaultAsync(i => i.Id.Equals(id), cancellationToken);
            return item;
        }

        public virtual async Task<TEntity> GetFirstItemAsync(CancellationToken cancellationToken = default)
        {
            var firstItem = await _context.Set<TEntity>().FirstAsync(cancellationToken);
            return firstItem;
        }

        public virtual async Task<TEntity> GetFirstItemAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var firstItem = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
            return firstItem;
        }

        public virtual async Task<IEnumerable<TEntity>> GetItemsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var items = await _context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
            return items;
        }

        public virtual async Task<TEntity> GetLastItemAsync(CancellationToken cancellationToken = default)
        {
            var lastItem = await _context.Set<TEntity>().OrderByDescending(e => e.Id).FirstOrDefaultAsync(cancellationToken);
            return lastItem;
        }

        public virtual async Task<TEntity> GetLastItemAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var lastItem = await _context.Set<TEntity>().OrderByDescending(e => e.Id).FirstOrDefaultAsync(predicate, cancellationToken);
            return lastItem;
        }

        public virtual async Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var exists = await _context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
            return exists;
        }
    }
}
