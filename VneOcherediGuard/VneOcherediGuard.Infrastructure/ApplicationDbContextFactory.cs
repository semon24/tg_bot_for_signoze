using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VneOcherediGuard.Options;

namespace VneOcherediGuard.Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();


            var dbOptions = new DataBaseOptions
            {
                Host = configuration["DataBase:Host"] ?? string.Empty,
                Port = configuration["DataBase:Port"] ?? string.Empty,
                Login = configuration["DataBase:Login"] ?? string.Empty,
                Password = configuration["DataBase:Password"] ?? string.Empty
            };
            var connectionString = $"Host={dbOptions.Host};Port={dbOptions.Port};Database=alerts;Username={dbOptions.Login};Password={dbOptions.Password}";
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options, dbOptions);
        }
    }
}
