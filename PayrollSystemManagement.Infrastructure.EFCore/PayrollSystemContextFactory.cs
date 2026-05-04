using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PayrollSystemManagement.Infrastructure.EFCore
{
    public class PayrollSystemContextFactory : IDesignTimeDbContextFactory<PayrollSystemContext>
    {
        public PayrollSystemContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<PayrollSystemContext>()
                .UseSqlServer(config.GetConnectionString("TaadolDb"))
                .Options;

            return new PayrollSystemContext(options);
        }
    }
}
