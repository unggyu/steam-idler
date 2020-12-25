using Microsoft.EntityFrameworkCore;
using SteamIdler.Infrastructure.Models;
using System;

namespace SteamIdler.Infrastructure.Contexts
{
    public class IdlerContext : DbContext
    {
        private static IdlerContext _instance;

        public static IdlerContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IdlerContext();
                }

                return _instance;
            }
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<AccountApp> AccountApps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite("Data Source=idler.db")
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>().ToTable(nameof(Accounts).ToUnderscoreCase());
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id).HasColumnName(nameof(Account.Id).ToUnderscoreCase());
                entity.Property(a => a.Username).HasColumnName(nameof(Account.Username).ToUnderscoreCase());
                entity.Property(a => a.Password).HasColumnName(nameof(Account.Password).ToUnderscoreCase());
                entity.Property(a => a.LoginKey).HasColumnName(nameof(Account.LoginKey).ToUnderscoreCase());
                entity.Property(a => a.SentryFilePath).HasColumnName(nameof(Account.SentryFilePath).ToUnderscoreCase());
            });

            modelBuilder.Entity<App>().ToTable(nameof(Apps).ToUnderscoreCase());
            modelBuilder.Entity<App>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id).HasColumnName(nameof(App.Id).ToUnderscoreCase());
            });

            modelBuilder.Entity<AccountApp>().ToTable(nameof(AccountApps).ToUnderscoreCase());
            modelBuilder.Entity<AccountApp>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Id).HasColumnName(nameof(AccountApp.Id).ToUnderscoreCase());
                entity.Property(a => a.AccountId).HasColumnName(nameof(AccountApp.AccountId).ToUnderscoreCase());
                entity.Property(a => a.AppId).HasColumnName(nameof(AccountApp.AppId).ToUnderscoreCase());

                entity.HasOne(a => a.Account)
                      .WithMany(a => a.AccountApps)
                      .HasForeignKey(a => a.AccountId);

                entity.HasOne(a => a.App)
                      .WithMany(a => a.AccountApps)
                      .HasForeignKey(a => a.AppId);
            });
        }
    }
}
