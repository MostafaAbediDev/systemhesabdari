using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BankManagement.Infrastructure.EFCore
{
    public class BankFakeDataContextFactory : IDesignTimeDbContextFactory<BankFakeDataContext>
    {
        public BankFakeDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<BankFakeDataContext>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new BankFakeDataContext(options);
        }
    }
}
