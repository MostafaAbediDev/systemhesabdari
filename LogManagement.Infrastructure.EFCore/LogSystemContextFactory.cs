using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LogManagement.Infrastructure.EFCore
{
    public class LogSystemContextFactory : IDesignTimeDbContextFactory<LogSystemContext>
    {
        public LogSystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<LogSystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new LogSystemContext(options);
        }
    }
}
