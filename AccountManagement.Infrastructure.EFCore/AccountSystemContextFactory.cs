using AccountManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PayrollSystemManagement.Infrastructure.EFCore
{
    public class AccountSystemContextFactory : IDesignTimeDbContextFactory<AccountSystemContext>
    {
        public AccountSystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<AccountSystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new AccountSystemContext(options);
        }
    }
}
