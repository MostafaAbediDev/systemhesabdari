using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PayrollSystemManagement.Infrastructure.EFCore
{
    public class PayrollFakeDataContextFactory : IDesignTimeDbContextFactory<PayrollFakeDataContext>
    {
        public PayrollFakeDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<PayrollFakeDataContext>()
                .UseSqlServer(config.GetConnectionString("TaadolFakeDb"))
                .Options;

            return new PayrollFakeDataContext(options);
        }
    }
}
