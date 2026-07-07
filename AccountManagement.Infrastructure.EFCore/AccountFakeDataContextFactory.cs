using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AccountManagement.Infrastructure.EFCore
{
    public class AccountFakeDataContextFactory : IDesignTimeDbContextFactory<AccountFakeDataContext>
    {
        public AccountFakeDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<AccountFakeDataContext>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new AccountFakeDataContext(options);
        }
    }
}
