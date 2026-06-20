using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinancialManagement.Infrastructure.EFCore
{
    public class FinancialFakeDataContextFactory : IDesignTimeDbContextFactory<FinancialFakeDataContext>
    {
        public FinancialFakeDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<FinancialFakeDataContext>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new FinancialFakeDataContext(options);
        }
    }
}
