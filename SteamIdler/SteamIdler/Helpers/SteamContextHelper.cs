using Microsoft.EntityFrameworkCore;
using SteamIdler.Context;
using SteamIdler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SteamIdler.Helpers
{
    public class SteamContextHelper<T> where T : SteamContext
    {
        protected T CreateContext()
        {
            var steamContext = (T)Activator.CreateInstance(typeof(T));
            steamContext.Database.EnsureCreated();
            steamContext.Database.Migrate();

            return steamContext;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            using (var context = CreateContext())
            {
                return await context.Accounts
                    .AsNoTracking()
                    .OrderByDescending(a => a.Username)
                    .ToListAsync();
            }
        }

        public async Task AddOrUpdateAccountsAsync(List<Account> accounts, CancellationToken token = default)
        {
            using (var context = CreateContext())
            {
                var newAccounts = from a in accounts
                                  where context.Accounts.Any(aa => aa.Username.Equals(a.Username) == false)
                                  select a;

                await context.Accounts.AddRangeAsync(newAccounts, token);
                await context.SaveChangesAsync(token);
            }
        }
    }
}
