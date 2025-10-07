using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using VneOcherediGuard.DataBase;
using VneOcherediGuard.Infrastructure.DataBase;
using VneOcherediGuard.Options;


namespace VneOcherediGuard.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ChatIds> ChatIds { get; set; }
        public DbSet<MessagesTime> MessagesTime { get; set; }

        private readonly string _passwordDataBase;
        private readonly string _loginDataBase;
        private readonly string _hostDataBase;
        private readonly string _portDataBase;


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, DataBaseOptions dataBaseOptions)
        : base(options)
        {
            _passwordDataBase = dataBaseOptions.Password;
            _loginDataBase = dataBaseOptions.Login;
            _hostDataBase = dataBaseOptions.Host;
            _portDataBase = dataBaseOptions.Port;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql($"Host={_hostDataBase};Port={_portDataBase};Database=alerts;Username={_loginDataBase};Password={_passwordDataBase}");
            }
        }
    }
}
 