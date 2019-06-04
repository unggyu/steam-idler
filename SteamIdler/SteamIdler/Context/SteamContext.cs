using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using SteamIdler.Models;
using System;
using System.IO;
using Xamarin.Forms;

namespace SteamIdler.Context
{
    public class SteamContext : DbContext
    {
        private const string _databaseName = "steam.db";

        public DbSet<Account> Accounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Batteries_V2.Init();
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", _databaseName);
                    break;
                case Device.Android:
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), _databaseName);
                    break;
                case Device.UWP:
                    databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SteamIdler", _databaseName);
                    break;
                default: throw new NotImplementedException("Platform not supported");
            }

            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }
    }
}
